using LiveLarson.BootAndLoad;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    private string nextSceneName = "Loading";
    
    private void Awake()
    {
        LoadInOrder();
    }
    
    private void LoadInOrder()
    {
        Addressables.LoadAssetAsync<GameObject>("GameSystem").Completed += op0 =>
        {
            var go0 = Instantiate(op0.Result);
            DontDestroyOnLoad(go0);
            Addressables.LoadAssetAsync<GameObject>("ApplicationContext").Completed += op1 =>
            {
                var go1 = Instantiate(op1.Result);
                DontDestroyOnLoad(go1);
                Addressables.LoadAssetAsync<GameObject>("DataTableManager").Completed += op2 =>
                {
                    var go2 = Instantiate(op2.Result);
                    DontDestroyOnLoad(go2);
                    ApplicationContext.Instance.LoadScene(nextSceneName);
                };
            };
        };
    }
}