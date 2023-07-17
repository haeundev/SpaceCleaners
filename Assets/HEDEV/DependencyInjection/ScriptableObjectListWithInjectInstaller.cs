using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Proto.DependencyInjection
{
    [CreateAssetMenu(fileName = "ScriptableObjectListWithInjectInstaller", menuName = "Installers/ScriptableObjectListWithInjectInstaller")]
    public class ScriptableObjectListWithInjectInstaller : ScriptableObjectInstaller<ScriptableObjectListWithInjectInstaller>
    {
        [SerializeField] protected List<ScriptableObject> scriptableObjects;

        public override void InstallBindings()
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                var componentType = scriptableObject.GetType();
                Container.Bind(componentType.Interfaces().Concat(new[] { componentType }).ToArray())
                    .FromInstance(scriptableObject).AsSingle().NonLazy();
                Container.QueueForInject(scriptableObject);
            }
        }
    }
}