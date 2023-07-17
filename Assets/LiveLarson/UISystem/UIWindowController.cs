using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveLarson.UISystem
{
    public class UIWindowController : MonoBehaviour
    {
        [SerializeField] private UIContainer uiContainer;
        private Dictionary<Type, string> _mPathContainer;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            InitContainer();
        }

        private void InitContainer()
        {
            _mPathContainer = new Dictionary<Type, string>();

            foreach (var uiKeyValue in uiContainer.uis) _mPathContainer[uiKeyValue.Window.GetType()] = uiKeyValue.path;
        }

        public string Find<T>() where T : UIWindow
        {
            string value = null;
            _mPathContainer.TryGetValue(typeof(T), out value);
            return value;
        }
    }
}