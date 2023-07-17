using System.IO;
using LiveLarson.UISystem.Editor;
using UnityEditor;
using UnityEngine;

namespace LiveLarson.Util.Editor
{
    public static class ContextMenu
    {
        [MenuItem("CONTEXT/MonoBehaviour/Create or Edit Editor script")]
        private static void CreateEditorScript(MenuCommand menuCommand)
        {
            MonoScript script;
            if (menuCommand.context is MonoBehaviour mono)
            {
                script = MonoScript.FromMonoBehaviour(mono);
            }
            else if (menuCommand.context is ScriptableObject scriptableObject)
            {
                script = MonoScript.FromScriptableObject(scriptableObject);
            }
            else
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(script);
            if (string.IsNullOrEmpty(path))
                return;

            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
                return;

            var className = Path.GetFileNameWithoutExtension(path);
            var targetName = className + "Editor";
            var targetDirectory = Path.Combine(directory, "Editor");
            var targetPath = Path.Combine(targetDirectory, targetName + ".cs");
            Directory.CreateDirectory(targetDirectory);
            if (!File.Exists(targetPath))
            {
                using (var writer = new StreamWriter(targetPath))
                {
                    writer.WriteLine("using UnityEngine;");
                    writer.WriteLine("using UnityEditor;");
                    writer.WriteLine("");
                    writer.WriteLine($"[CustomEditor(typeof({className}))]");
                    writer.WriteLine($"public class {targetName} : Editor");
                    writer.WriteLine("{");
                    writer.WriteLine("    public override void OnInspectorGUI()");
                    writer.WriteLine("    {");
                    writer.WriteLine("        base.OnInspectorGUI();");
                    writer.WriteLine("");
                    writer.WriteLine("        if (GUILayout.Button(\"Collect\"))");
                    writer.WriteLine("        {");
                    writer.WriteLine($"            var obj = target as {className};");
                    writer.WriteLine("            //obj.Collect();");
                    writer.WriteLine("            EditorUtility.SetDirty(obj);");
                    writer.WriteLine("        }");
                    writer.WriteLine("    }");
                    writer.WriteLine("}");
                    writer.WriteLine("");
                    writer.Close();
                }

                AssetDatabase.Refresh();
            }

            AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(targetPath), 12);
        }

        [MenuItem("CONTEXT/UIWindow/Open UI script maker")]
        private static void OpenUIScriptMaker(MenuCommand menuCommand)
        {
            var mono = menuCommand.context as UISystem.UIWindow;
            if (mono == null)
                return;

            UIWindowScriptMaker.OpenWindow(mono);
        }

        [MenuItem("GameObject/Utils/Space to Underscore Recursive", false, 0)]
        static void Test(MenuCommand menuCommand)
        {
            var obj = menuCommand.context as GameObject;
            if (obj != null)
                ConvertSpaceToUnderscoreRecursive(obj.transform);
        }

        static void ConvertSpaceToUnderscoreRecursive(Transform tran)
        {
            for (var i = 0; i < tran.childCount; i++)
            {
                var child = tran.GetChild(i);
                if (child != null)
                    ConvertSpaceToUnderscoreRecursive(child);
            }

            tran.name = tran.name.ConvertSpaceToUnderScore();
        }

        [MenuItem("CONTEXT/MonoBehaviour/Remove from all Prefabs and Scenes")]
        private static void RemoveScriptFromAllObject(MenuCommand menuCommand)
        {
            var mono = menuCommand.context as MonoBehaviour;
            if (mono == null)
                return;

            var type = mono.GetType();
            DeleteInScene(type);
        }

        public static void DeleteInScene(System.Type type)
        {
            var all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            for (var i = 0; i < all.Length; ++i)
            {
                var obj = all[i];
                if (obj == null)
                    continue;
                Object.DestroyImmediate(obj.GetComponent(type), true);
            }
        }

        public static void DeleteInPrefab(System.Type type)
        {
            var allGuid = AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < allGuid.Length; ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(allGuid[i]);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                Object.DestroyImmediate(obj.GetComponent(type), true);
            }

            AssetDatabase.Refresh();
        }
    }
}