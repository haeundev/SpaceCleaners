using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Proto.Booting
{
    public class DontDestroyBootTarget : BootTarget
    {
        [SerializeField] private AssetReferenceGameObject target;

        protected GameObject TargetInstance { get; set; }

        protected override async Task Load()
        {
            var handle = target.LoadAssetAsync<GameObject>();
            await handle.Task;
            TargetInstance = Instantiate(handle.Result);
        }

        protected override void Configure()
        {
            DontDestroyOnLoad(TargetInstance);
        }
    }
}