using System;
using UnityEngine;

namespace LiveLarson.Camera
{
    public static class CameraManager
    {
        public enum CameraType
        {
            Main,
            UI
        }

        private static UnityEngine.Camera _main;
        private static UnityEngine.Camera _ui;

        public static UnityEngine.Camera Main
        {
            get
            {
                _main ??= UnityEngine.Camera.main;
                _main ??= GameObject.FindWithTag("MainCamera")?.GetComponent<UnityEngine.Camera>();
                return _main;
            }
        }

        public static UnityEngine.Camera UI
        {
            get
            {
                _ui ??= GameObject.FindWithTag("UICamera")?.GetComponent<UnityEngine.Camera>();
                return _ui;
            }
        }

        public static void Reset()
        {
            _main = null;
            _ui = null;
        }

        public static UnityEngine.Camera Get(CameraType type)
        {
            return type switch
            {
                CameraType.Main => Main,
                CameraType.UI => UI,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}