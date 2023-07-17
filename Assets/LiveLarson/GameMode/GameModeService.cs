using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiveLarson.Camera;
using LiveLarson.Camera.CameraFade;
using LiveLarson.SoundSystem;
using LiveLarson.UISystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace LiveLarson.GameMode
{
    public class GameModeService : MonoBehaviour, IGameModeService
    {
        private GameModeService Instance { get; set; }
        private const float DEFAULT_FADE_TIME = 0.3f;
        private static readonly List<Enums.GameMode> BaseGameModes = new() { Enums.GameMode.InGame };
        private readonly Stack<Enums.GameMode> _gameModeHistoryStack = new();
        private readonly IUiServiceFactory _uiServiceFactory;
        private Enums.GameMode _gameMode;
        private Enums.GameMode _nextGameMode;

        public GameModeService(IUiServiceFactory uiServiceFactory)
        {
            _uiServiceFactory = uiServiceFactory;
        }

        private void Awake()
        {
            Instance = this;
        }

        public static float FadeTime { get; set; } = DEFAULT_FADE_TIME;
        public static bool IsReadyToFadeIn { get; set; }
        public event Action<Enums.GameMode> OnGameModeEnter = delegate { };
        public event Action<Enums.GameMode> OnGameModeExit = delegate { };

        public Enums.GameMode GetGameMode()
        {
            return _gameMode;
        }

        public Enums.GameMode GetNextGameMode()
        {
            return _nextGameMode;
        }

        public void EnterLoadingGameMode(Enums.GameMode gameMode)
        {
            throw new NotImplementedException();
        }

        public void EnterGameMode(Enums.GameMode gameMode)
        {
            if (_gameMode == gameMode)
                return;
            _gameModeHistoryStack.Push(_gameMode);

            SetGameMode(gameMode);
        }

        public void LeaveGameMode(Enums.GameMode gameMode)
        {
            Assert.AreEqual(gameMode, _gameMode, $"Your game mode is not {gameMode} but {_gameMode}.");
            Assert.IsTrue(_gameModeHistoryStack.Count > 0,
                "The game mode history is empty. Make sure you entered a game mode.");
            var lastGameMode = _gameModeHistoryStack.Pop();
            if (lastGameMode == Enums.GameMode.None)
            {
                lastGameMode = Enums.GameMode.InGame;
                _gameModeHistoryStack.Push(Enums.GameMode.None);
            }

            SetGameMode(gameMode);
        }

        public void ResetGameMode()
        {
            while (_gameModeHistoryStack.Count > 0)
            {
                var stackGameMode = _gameModeHistoryStack.Pop();

                OnGameModeExit?.Invoke(stackGameMode);
                if (_uiServiceFactory.GetUiService(stackGameMode) != null)
                    _uiServiceFactory.GetUiService(stackGameMode).Exit();
                SoundService.Instance.OnGameModeExit(stackGameMode);
            }

            CameraManager.Reset();
            _gameModeHistoryStack.Push(Enums.GameMode.InGame);
        }

        public void ForceSetGameMode(Enums.GameMode gameMode)
        {
            _gameModeHistoryStack.Clear();
            SetGameMode(gameMode);
        }

        public bool IsCurrentlyInBaseGameMode(Enums.GameMode mode)
        {
            return (from gameMode in _gameModeHistoryStack
                where IsClassifiedAsBaseGameMode(gameMode)
                select mode == gameMode).FirstOrDefault();
        }

        public bool IsClassifiedAsBaseGameMode(Enums.GameMode mode)
        {
            return BaseGameModes.Contains(mode);
        }

        private void SetGameMode(Enums.GameMode gameMode)
        {
            _nextGameMode = gameMode;
            Debug.Log($"SetGameMode : {gameMode}");
            CameraManager.Reset();
            // Leave game mode
            OnGameModeExit?.Invoke(_gameMode);
            if (_uiServiceFactory.GetUiService(_gameMode) != null)
                _uiServiceFactory.GetUiService(_gameMode).Exit();
            SoundService.Instance.OnGameModeExit(_gameMode);

            _gameMode = gameMode;
            if (_uiServiceFactory.GetUiService(_gameMode) != null)
                _uiServiceFactory.GetUiService(_gameMode).Enter();
            SoundService.Instance.OnGameModeEnter(_gameMode);

            OnGameModeEnter?.Invoke(_gameMode);
        }

        private IEnumerator ChangeWithFade(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(FadeTime);
            yield return new WaitForSeconds(FadeTime);
            yield return null;
            while (!IsReadyToFadeIn)
                yield return null;

            SetGameMode(gameMode);
            yield return null;
            CameraFadeService.In(FadeTime);
        }

        private IEnumerator ChangeWithFadeOut(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(DEFAULT_FADE_TIME);
            yield return new WaitForSeconds(DEFAULT_FADE_TIME);
            SetGameMode(gameMode);
            yield return null;
            CameraFadeService.In(0);
        }

        private IEnumerator ChangeWithFadeIn(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(0);
            yield return null;
            SetGameMode(gameMode);
            yield return new WaitForSeconds(DEFAULT_FADE_TIME);
            CameraFadeService.In(DEFAULT_FADE_TIME);
        }
    }
}