using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace LiveLarson.SceneSystem
{
    public class SceneChanger : MonoBehaviour
    {
        private static SceneChanger _instance;

        public static SceneChanger Instance
        {
            get
            {
                if (_instance.IsNull())
                {
                    _instance = FindObjectOfType<SceneChanger>();
                    if (_instance.IsNull()) _instance = new GameObject("SceneChanger").AddComponent<SceneChanger>();
                }

                return _instance;
            }
        }

        private static bool _isLoading;
        private readonly Dictionary<string, SceneInstance> _loadedScenes = new();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            DontDestroyOnLoad(this);
        }

        public static void AddScene(string scenePath, LoadSceneMode mode,
            Action<AsyncOperationHandle<SceneInstance>> onComplete = null)
        {
            if (_isLoading)
                return;

            Instance.StartCoroutine(Instance.LoadSceneAsync(scenePath, mode, true, onComplete));
        }

        public static IEnumerator AddScene(string scene)
        {
            var handle = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return handle;
            Instance._loadedScenes.Add(scene, handle.Result);
        }

        private IEnumerator LoadSceneAsync(string scenePath, LoadSceneMode loadSceneMode, bool activeOnLoad,
            Action<AsyncOperationHandle<SceneInstance>> onComplete)
        {
            _isLoading = true;
            yield return null;
            var operation = Addressables.LoadSceneAsync(scenePath, loadSceneMode, activeOnLoad);
            if (onComplete != null)
                operation.Completed += onComplete;
            yield return operation;
            _isLoading = false;
        }

        public static void RemoveScene(AsyncOperationHandle<SceneInstance> handle)
        {
            Addressables.UnloadSceneAsync(handle);
        }

        public static IEnumerator RemoveScene(string scene)
        {
            var found = Instance._loadedScenes.TryGetValue(scene, out var sceneInstance);
            if (!found) yield break;
            yield return Addressables.UnloadSceneAsync(sceneInstance);
            Instance._loadedScenes.Remove(scene);
        }
    }
}