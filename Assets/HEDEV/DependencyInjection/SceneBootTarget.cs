using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Proto.Booting
{
    public class SceneBootTarget : BootTarget
    {
        [SerializeField] private string sceneName = null;
        [SerializeField] private LoadSceneMode mode = LoadSceneMode.Single;
        [SerializeField] private bool activateOnLoad = true;
    
        protected override Task Load()
        {
            var handle = Addressables.LoadSceneAsync(sceneName, mode, activateOnLoad);
            handle.Completed += OnSceneLoaded;
            return handle.Task;
        }

        private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> instance)
        {
          if (!activateOnLoad)
              instance.Result.ActivateAsync();

            //////ApplicationContext.Instance.ShowDisplayLoading(false);
        }
    }
}