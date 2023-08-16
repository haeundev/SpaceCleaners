using LiveLarson.BootAndLoad;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    public enum BootTarget
    {
        GameSystem, ApplicationContext, DataTableManager
    }
    
    [SerializeField] private string nextSceneName = "Booting_1_ApplicationContext";
    [SerializeField] private BootTarget bootTarget = BootTarget.GameSystem;
    
    private void Awake()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/LiveLarson/DataTableManagement/DataTableManager.prefab").Completed += op2 =>
        {
            var go2 = Instantiate(op2.Result);
            DontDestroyOnLoad(go2);
            ApplicationContext.Instance.LoadScene(nextSceneName);
        };
    }
    
    private void LoadInOrder()
    {
        // AddressableHelper.LoadGameObject("GameSystem");
        Addressables.LoadAssetAsync<GameObject>("Assets/LiveLarson/BootAndLoad/GameSystem.prefab").Completed += op0 =>
        {
            var go0 = Instantiate(op0.Result);
            DontDestroyOnLoad(go0);
            Addressables.LoadAssetAsync<GameObject>("Assets/LiveLarson/BootAndLoad/ApplicationContext.prefab").Completed += op1 =>
            {
                var go1 = Instantiate(op1.Result);
                DontDestroyOnLoad(go1);
              
            };
        };
    }
}