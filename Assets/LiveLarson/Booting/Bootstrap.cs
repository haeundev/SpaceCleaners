using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LiveLarson.Booting
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private List<BootTarget> targets = default;

        private void Awake()
        {
            Run();
        }

        private async void Run()
        {
            foreach (var target in targets)
            {
                await target.Run();
            }
        }

        public IEnumerator Execute()
        {
            return targets.Select(target => target.Run()).GetEnumerator();
        }

        [Button]
        public void Collect()
        {
            targets = GetComponentsInChildren<BootTarget>().ToList();
        }
    }
}