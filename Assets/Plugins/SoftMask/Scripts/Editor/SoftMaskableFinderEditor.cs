using SoftMasking.Extensions;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoftMaskableFinder))]
public class SoftMaskableFinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Find"))
        {
            var obj = target as SoftMaskableFinder;
            obj.Find();
            EditorUtility.SetDirty(obj);
        }
        
        if (GUILayout.Button("Delete all"))
        {
            var obj = target as SoftMaskableFinder;
            obj.DeleteAll();
            EditorUtility.SetDirty(obj);
        }
    }
}

