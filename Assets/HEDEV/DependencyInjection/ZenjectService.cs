using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Proto.Service
{
    public class ZenjectService
    {
        private static DiContainer _diContainer = null;
        
        public ZenjectService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public static T Resolve<T>() where T : class => _diContainer?.Resolve<T>();
        public static T ResolveId<T>(object identifier) where T : class => _diContainer?.ResolveId<T>(identifier);
        public static void ResolveAll<T>() where T : class => _diContainer?.ResolveAll<T>();
        public static void InjectGameObject(GameObject gameObject) => _diContainer?.InjectGameObject(gameObject);

        public static T Instantiate<T>() where T : class => _diContainer?.Instantiate<T>();
        public static T Instantiate<T>(IEnumerable<object> extraArgs) where T : class => _diContainer?.Instantiate<T>(extraArgs);
        public static GameObject InstantiatePrefab(Object obj) => _diContainer?.InstantiatePrefab(obj);
        public static GameObject InstantiatePrefab(Object obj, Transform parentTransform) => _diContainer?.InstantiatePrefab(obj, parentTransform);
        public static T InstantiatePrefabForComponent<T>(Object prefab, Transform parentTransform) where T : Component => _diContainer?.InstantiatePrefabForComponent<T>(prefab, parentTransform);
    }
}
