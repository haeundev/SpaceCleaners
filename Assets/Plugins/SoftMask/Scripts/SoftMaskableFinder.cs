using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoftMasking.Extensions
{
    [Serializable]
    public class SoftMaskableData
    {
        public string Name;
        public Transform transform;
        public List<SoftMaskable> softMaskables;
    }
    
    public class SoftMaskableFinder : UIBehaviour
    {
        public List<SoftMaskableData> softMaskables;

        public void Find()
        {
            if (softMaskables == null)
                softMaskables = new List<SoftMaskableData>();
            softMaskables.Clear();
            Find(transform);
            Debug.Log("find done.");
        }

        private void Find(Transform root)
        {
            var maskables = root.GetComponents<SoftMaskable>();
            if (maskables != null && maskables.Length > 0)
            {
                var data = new SoftMaskableData();
                data.Name = root.name;
                data.transform = root;
                data.softMaskables = maskables.ToList();
                softMaskables.Add(data);
            }
            ForEachChild(root, Find);
        }
        
        public static void ForEachChild(Transform transform, Action<Transform> action)
        {
            if (transform == null)
                return;

            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = transform.GetChild(i);
                action?.Invoke(child);
            }
        }

        public void DeleteAll()
        {
            softMaskables.ForEach(p => p.softMaskables.ForEach(DestroyImmediate));
            Debug.Log("delete done.");
        }
    }
}