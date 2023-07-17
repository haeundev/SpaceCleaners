using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.GameMode;
using LiveLarson.GamePause;
using LiveLarson.SoundSystem;
using LiveLarson.UISystem;
using LiveLarson.Util;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
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
        [SerializeField] private Transform playerSpot;
        [SerializeField] private LoadingDisplay[] displays;
        [SerializeField] private bool isLoadingSceneDebugMode;
        [SerializeField] private LoadingDisplay.LoadingThemeType loadingThemeType;
        
        private LoadingDisplay _currentDisplay;
        private string _previousSceneName;
    
        private static ApplicationContext _instance;
        
        private readonly Dictionary<string, SceneInstance> _subScenes = new();
        
        public static ApplicationContext Instance => _instance;

        private bool _loading;
        public bool IsLoadingDisplayOn { get; private set; }
        
#if UNITY_EDITOR
        public static event Action OnReadyToLoad = delegate { };
        public static event Action OnLoadCompleted = delegate { };
#endif        

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Random.InitState(DateTime.Now.Millisecond);
        }

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == default)
            {
                Debug.LogError("No player!");
            }
            player.transform.position = playerSpot.position;
            player.transform.rotation = playerSpot.rotation;
        }

        public void LoadScene(string scene, UnityAction loaded = null, UnityAction activated = null)
        {
            if (_loading)
                return;

            GameModeService.EnterGameMode(Enums.GameMode.Loading);
            // SoundService.Play("UI/Common/Common_Loading_Loop");
            _currentDisplay = displays[0]; // 그 외의 경우들은 임시로 Common 로딩
            
            _loading = true;
            Debug.Log(scene);

            StartCoroutine(LoadSceneAsync(scene, loaded, activated));
        }

        public void LoadScene(string scene, Enums.GameMode mode, UnityAction loaded = null, UnityAction activated = null)
        {
            if (_loading)
                return;

            var prevGameMode = GameModeService.GetGameMode();
            GameModeService.EnterLoadingGameMode(mode);
            
            SoundService.Play("UI/Common/Common_Loading_Loop");
            ChangeLoadingThema(prevGameMode, mode);
            _currentDisplay.gameObject.SetActive(true);

            _loading = true;
            Debug.Log(scene);

            StartCoroutine(LoadSceneAsync(scene, loaded, activated));
        }

        private void ChangeLoadingThema(Enums.GameMode prev, Enums.GameMode next)
        {
            _currentDisplay.gameObject.SetActive(false);
            if (isLoadingSceneDebugMode)
            {
                _currentDisplay = displays[(int)loadingThemeType];
                return;
            }
            _currentDisplay = displays[2];
        }
        
        private IEnumerator LoadSceneAsync(string scene, UnityAction loaded, UnityAction activated)
        {
#if UNITY_EDITOR
            OnReadyToLoad();
#endif
            _currentDisplay.UnityPreviousSceneName = _previousSceneName;
            _currentDisplay.UnitySceneName = scene;
            _previousSceneName = scene;
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
                yield return null;
            }

            while (progress < 1.0f)
            {
                progress = Mathf.MoveTowards(progress, 1.0f, Time.unscaledDeltaTime);
                _currentDisplay.UpdateProgress(progress);
                yield return null;
            }

            while (Time.unscaledTime - startLoadingTime < waitingTime)
            { 
                yield return null;
            }

            loaded?.Invoke();
            ShowDisplayLoading(false);

            yield return loader.Result.ActivateAsync();
           

            activated?.Invoke();

            this.DoWaitForSeconds(1.0f, ChangeGameMode);

#if UNITY_EDITOR
            OnLoadCompleted();
#endif
            _loading = false;
        }

        private void ChangeGameMode()
        {
            GameModeService.LeaveGameMode(Enums.GameMode.Loading);
            _currentDisplay.gameObject.SetActive(false);
            IsLoadingDisplayOn = false;
        }
        
        public void AddScene(string scene, UnityAction complete)
        {
            StartCoroutine(scene, complete);
        }
        
        private IEnumerator AddSceneAsync(string scene, UnityAction complete)
        {
            var loader = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive, false);
            yield return loader;
            complete?.Invoke();
            _subScenes.Add(scene, loader.Result);
            yield return loader.Result.ActivateAsync();
        }
        
        public void RemoveScene(string scene, UnityAction complete = null)
        {
            if (_subScenes.TryGetValue(scene, out var instance))
            {
                Observable.FromCoroutine(() => Addressables.UnloadSceneAsync(instance))
                    .Subscribe(_ => { },
                        () =>
                        {
                            complete?.Invoke();
                            _subScenes.Remove(scene);
                        })
                    .AddTo(this);
            }
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
            //UnlockManager.Reset();
        }
    }
}