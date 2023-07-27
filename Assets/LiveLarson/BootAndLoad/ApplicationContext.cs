using System;
using System.Collections;
using LiveLarson.GameMode;
using LiveLarson.GamePause;
using LiveLarson.UISystem;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace LiveLarson.BootAndLoad
{
    public class ApplicationContext : MonoBehaviour
    {
        private GameModeService _gameModeService;
        public GameModeService GameModeService
        {
            get
            {
                if (_gameModeService == default)
                    _gameModeService = FindObjectOfType<GameModeService>();
                return _gameModeService;
            }
        }
        [SerializeField] private LoadingDisplay[] displays;
        [SerializeField] private LoadingDisplay.LoadingThemeType loadingThemeType;
        private LoadingDisplay _currentDisplay;
        public static ApplicationContext Instance { get; private set; }

        private bool _loading;
        private Enums.GameMode _previousMode;
        public bool IsLoadingDisplayOn { get; private set; }
        
#if UNITY_EDITOR
        public static event Action OnReadyToLoad = delegate { };
        public static event Action OnLoadCompleted = delegate { };
#endif
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Random.InitState(DateTime.Now.Millisecond);
        }
        
        public void LoadScene(string scene, UnityAction loaded = null, UnityAction activated = null)
        {
            if (_loading)
                return;
            var targetGameMode = Enum.Parse<Enums.GameMode>(scene.Replace("Assets/Scenes/", "").Replace(".unity", ""));
            GameModeService.TryEnterGameMode(targetGameMode);
            // SoundService.Play("UI/Common/Common_Loading_Loop");
            _currentDisplay = displays[0]; // 그 외의 경우들은 임시로 Common 로딩
            _loading = true;
            StartCoroutine(LoadSceneAsync(scene, targetGameMode, loaded, activated));
        }
        
        private IEnumerator LoadSceneAsync(string scene, Enums.GameMode gameMode, UnityAction loaded, UnityAction activated)
        {
#if UNITY_EDITOR
            OnReadyToLoad();
#endif
            _currentDisplay.gameObject.SetActive(true);
            IsLoadingDisplayOn = true;
            yield return null;

            var loader = Addressables.LoadSceneAsync(scene, LoadSceneMode.Single, false);
            var baseOffset = loader.PercentComplete;
            var progress = 0.0f;
            var waitingTime = 3.0f;
            var startLoadingTime = Time.unscaledTime;

            while (!loader.IsDone)
            {
                progress = (loader.PercentComplete - baseOffset) * 10;
                _currentDisplay.UpdateProgress(progress);
                yield return default;
            }
            while (progress < 1.0f)
            {
                progress = Mathf.MoveTowards(progress, 1.0f, Time.unscaledDeltaTime);
                _currentDisplay.UpdateProgress(progress);
                yield return default;
            }
            while (Time.unscaledTime - startLoadingTime < waitingTime)
            { 
                yield return default;
            }

            loaded?.Invoke();
            ShowDisplayLoading(false);

            yield return loader.Result.ActivateAsync();

            activated?.Invoke();

            this.DoWaitForSeconds(1f, () => OnLoadSceneAsyncDone(gameMode));

#if UNITY_EDITOR
            OnLoadCompleted();
#endif
            _loading = false;
        }

        private void OnLoadSceneAsyncDone(Enums.GameMode gameMode)
        {
            Debug.Log($"ChangeGameMode to {gameMode}");
            _currentDisplay.gameObject.SetActive(false);
            IsLoadingDisplayOn = false;
            // GameModeService.TryEnterGameMode(gameMode);
            _previousMode = gameMode;
        }
        
        public void ShowDisplayLoading(bool show)
        {
            _currentDisplay = displays[0];
            _currentDisplay.gameObject.SetActive(show);
            IsLoadingDisplayOn = show;
            _currentDisplay.UpdateProgress(0.0f);
        }
        
        private void ResetData()
        {
            UIWindowService.CloseAll();
            GamePauseManager.AllResume();
        }
    }
}