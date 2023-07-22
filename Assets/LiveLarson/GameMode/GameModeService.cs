using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.Camera;
using LiveLarson.Camera.CameraFade;
using LiveLarson.SoundSystem;
using UnityEngine;

namespace LiveLarson.GameMode
{
    public class GameModeService : MonoBehaviour, IGameModeService
    {
        private GameModeService Instance { get; set; }
        private const float DEFAULT_FADE_TIME = 0.3f;
        private static readonly List<Enums.GameMode> BaseGameModes = new() { Enums.GameMode.OuterSpace };
        private Enums.GameMode _currentGameMode;
        
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
            return _currentGameMode;
        }

        public void TryEnterGameMode(Enums.GameMode gameMode)
        {
            if (_currentGameMode == gameMode)
            {
                Debug.Log($"already in gameMode {gameMode.ToString()}");
                return;
            }
            Debug.Log($"EnterGameMode {gameMode.ToString()}");
            ChangeGameMode(gameMode);
        }
        
        private void ChangeGameMode(Enums.GameMode targetGameMode)
        {
            Debug.Log($"SetGameMode : {targetGameMode}");
            CameraManager.Reset();
            // Leave game mode
            OnGameModeExit?.Invoke(_currentGameMode);
            SoundService.Instance.OnGameModeExit(_currentGameMode);
            _currentGameMode = targetGameMode;
            SoundService.Instance.OnGameModeEnter(_currentGameMode);
            OnGameModeEnter?.Invoke(_currentGameMode);
        }

        private IEnumerator ChangeWithFade(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(FadeTime);
            yield return new WaitForSeconds(FadeTime);
            yield return null;
            while (!IsReadyToFadeIn)
                yield return null;

            ChangeGameMode(gameMode);
            yield return null;
            CameraFadeService.In(FadeTime);
        }

        private IEnumerator ChangeWithFadeOut(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(DEFAULT_FADE_TIME);
            yield return new WaitForSeconds(DEFAULT_FADE_TIME);
            ChangeGameMode(gameMode);
            yield return null;
            CameraFadeService.In(0);
        }

        private IEnumerator ChangeWithFadeIn(Enums.GameMode gameMode)
        {
            CameraFadeService.Out(0);
            yield return null;
            ChangeGameMode(gameMode);
            yield return new WaitForSeconds(DEFAULT_FADE_TIME);
            CameraFadeService.In(DEFAULT_FADE_TIME);
        }
    }
}