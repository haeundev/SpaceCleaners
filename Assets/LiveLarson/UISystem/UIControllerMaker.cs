#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LiveLarson.UISystem
{
    #region SubWindows

    public class UISubWindowBody : ScriptableObject
    {
        [System.Serializable]
        public class Node
        {
            public Object targetScript;
        }

        public List<Node> nodes = new();

        public void CreateNewNode()
        {
            var node = new Node();
            nodes.Add(node);
        }

        public void DeleteNode(Node node)
        {
            if (nodes.Contains(node))
            {
                nodes.Remove(node);
            }
        }
    }

    #endregion

    public class UIControllerMaker : EditorWindow
    {
        private readonly string _defaultNamespaceName = "UI";
        private const string SCRIPT_PATH = "Assets/Scripts/UI/UIControllers/";
        public GameObject targetUIWindowObject;

        private UISubWindowBody _subWindowBody;

        private string _mClassName = "";

        private string _uiWindowType = "";

        [MenuItem("Tools/UI/2. UI Controller Maker", false , 2)]
        public static void ShowWindow()
        {
            GetWindow(typeof(UIControllerMaker));
        }

        private void OnGUI()
        {
            ClassNameGUI();
            TargetUigui();
            SubWindowGUI();
            WriteGUI();
        }

        private void ClassNameGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("클래스 이름 : ");
            _mClassName = EditorGUILayout.TextField(_mClassName);
            EditorGUILayout.EndHorizontal();
        }

        private void TargetUigui()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target UI Window Prefab ");
            targetUIWindowObject =
                (GameObject) EditorGUILayout.ObjectField(targetUIWindowObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }

        private void SubWindowGUI()
        {
            if (_subWindowBody == null) _subWindowBody = CreateInstance<UISubWindowBody>();
            GUILayout.Label("Sub UI Controller");
            var editor = Editor.CreateEditor(_subWindowBody);
            editor.OnInspectorGUI();
        }

        private void WriteGUI()
        {
            if (GUILayout.Button("Build"))
            {
                WriteFile();
                WriteGeneratorFile();
            }
        }

        #region Wirte Class

        void WriteFile()
        {
            var path = SCRIPT_PATH + _mClassName + "_Controller.cs";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("");
                writer.WriteLine("namespace " + _defaultNamespaceName);
                writer.WriteLine("{");
                WriteClass(writer);
                writer.WriteLine("}");
                writer.WriteLine("");
                writer.Close();
            }
            AssetDatabase.Refresh();
        }

        private void WriteClass(StreamWriter writer)
        {
            _uiWindowType = targetUIWindowObject.GetComponent<UIWindow>().GetType().ToString();
            writer.WriteLine($"    public partial class {_mClassName}_Controller : UIController");
            writer.WriteLine("    {");
            writer.WriteLine("");
            WriteOnCompleteSetting(writer);
            writer.WriteLine("    }");
        }

        private void WriteOnCompleteSetting(StreamWriter writer)
        {
            writer.WriteLine("        protected override void Awake()");
            writer.WriteLine("        {");
            writer.WriteLine("            Show();");
            //writer.WriteLine("            m_completeWindowSetting?.Invoke(this);");
            writer.WriteLine("        }");
        }

        #endregion

        #region Generator

        private void WriteGeneratorFile()
        {
            var path = SCRIPT_PATH + _mClassName + "_Controller_Generator.cs";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine($"//This is Auto Generated Code, Don't modify this script.");
                writer.WriteLine("using System;");
                writer.WriteLine("using Proto.UISystem;");
                writer.WriteLine("");
                writer.WriteLine("namespace " + _defaultNamespaceName);
                writer.WriteLine("{");
                WriteGeneratorClass(writer);
                writer.WriteLine("}");
                writer.WriteLine("");
                writer.Close();
            }
            Debug.Log($"{path}  Create Complete");
            AssetDatabase.Refresh();
        }

        private void WriteGeneratorClass(StreamWriter writer)
        {
            _uiWindowType = targetUIWindowObject.GetComponent<UIWindow>().GetType().ToString();
            writer.WriteLine($"    //This is Auto Generated Code");
            writer.WriteLine($"    public partial class {_mClassName}_Controller : UIController");
            writer.WriteLine("    {");
            writer.WriteLine("");
            writer.WriteLine($"        public override System.Type UIWindowType => typeof({_uiWindowType});");
            writer.WriteLine($"        public {_uiWindowType} Window => window as {_uiWindowType};");
            WriteChildren(writer);
            WriteOpen(writer);
            writer.WriteLine("    }");
        }

        private void WriteChild(StreamWriter writer)
        {
            foreach (var subWindow in _subWindowBody.nodes)
            {
                var subWindowType = subWindow.targetScript.name;
                var typeName = subWindowType.ToLower();

                writer.WriteLine($"            _{typeName} = new {subWindowType}();");
                writer.WriteLine($"            AddChildBody(_{typeName});");
            }
        }

        private void WriteChildren(StreamWriter writer)
        {
            if (_subWindowBody.nodes.Count > 0)
            {
                foreach (var subWindow in _subWindowBody.nodes)
                {
                    var subWindowType = subWindow.targetScript.name;
                    writer.WriteLine($"        private {subWindowType} _{subWindowType.ToLower()};");
                }

                writer.WriteLine("        protected override void SetChildrenControllers()");
                writer.WriteLine("        {");
                WriteChild(writer);
                writer.WriteLine("        }");
            }
        }

        private void WriteOpen(StreamWriter writer)
        {
            writer.WriteLine("        public override void SetWindowOption(int id, Action<UIController> completeWindowSetting, WindowOption option)");
            writer.WriteLine("        {");
            writer.WriteLine("            base.SetWindowOption(id, completeWindowSetting, option);");
            writer.WriteLine($"           CreateWindow<{_uiWindowType}>();");
            writer.WriteLine("        }");
        }
        
        #endregion
    }
}
#endif