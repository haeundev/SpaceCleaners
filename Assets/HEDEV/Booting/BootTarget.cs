using System.Threading.Tasks;
using UnityEngine;

namespace Proto.Booting
{
    public abstract class BootTarget : MonoBehaviour
    {
        public async Task Run()
        {
            await Load();
            Configure();
        }

        protected abstract Task Load();

        protected virtual void Configure() { }
    }
}