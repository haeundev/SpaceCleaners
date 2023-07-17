using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LiveLarson.UISystem
{
    public class UIWindowPathMaker : MonoBehaviour
    {
#if UNITY_EDITOR
        public UIContainer containerSo;
        public List<UIContainer> joinContainerSOs;
        public List<UIWindow> windows = new();
        
        public void Collect()
        {
            if (containerSo == null) return;
            containerSo.uis = new List<UIKeyValue>();
            foreach (var window in windows)
            {
                var uiKeyValue = new UIKeyValue();
                try
                {
                    uiKeyValue.Window = SetPrefab(window.gameObject, out uiKeyValue.path);
                    if (FindKey(uiKeyValue.Window.GetType())) continue;
                    containerSo.uis.Add(uiKeyValue);
                    window.gameObject.SetActive(false);
                }
                catch
                {
                    Debug.LogError("Checked Console");
                    break;
                }
               
            }
            EditorUtility.SetDirty(containerSo);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void Join()
        {
            if (containerSo == null) return;
            if (joinContainerSOs == null) return;

            foreach (var container in joinContainerSOs)
            {
                foreach (var joinKeyValue in container.uis)
                {
                    if (FindKey(joinKeyValue.Window.GetType())) continue;
                    containerSo.uis.Add(joinKeyValue);
                }
            }
            EditorUtility.SetDirty(containerSo);
            AssetDatabase.SaveAssets();
        }

        private bool FindKey(Type type)
        {
            var index = 0;
            foreach (var keyValue in containerSo.uis)
            {
                index++;
                if (keyValue.Window.GetType() == type)
                {
                    Debug.Log("***** 중첩된 UI Type 따라서 해당 UI는 컨테이너에 등록되지 않았습니다 : [Index : " + index + " ][ Type : " +
                              type + " ]");
                    return true;
                }
            }

            return false;
        }

        private UIWindow SetPrefab(GameObject windowObject, out string path)
        {
            Debug.Log("Window Path : " +
                      AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(windowObject)));
            path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(windowObject));

            var findPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(windowObject)),
                typeof(GameObject));
            return findPrefab.GetComponent<UIWindow>();
        }
#endif
    }
}