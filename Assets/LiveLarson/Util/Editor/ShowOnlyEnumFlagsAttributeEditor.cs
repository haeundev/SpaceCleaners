using LiveLarson.UISystem;
using UnityEditor;
using UnityEngine;

namespace LiveLarson.Util.Editor
{
    [CustomPropertyDrawer(typeof(ShowOnlyEnumFlagsAttribute))]
    public class ShowOnlyEnumFlagsDrawer : PropertyDrawer
    {
        private float _propertyHeight;
        private string _stringToPrint = string.Empty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
                return 0f;

            var strings = ((WindowOption)property.intValue).ToString().Split(',');
            _stringToPrint = string.Join("\n", strings).Replace("\n ", "\n");
            return _propertyHeight = EditorGUIUtility.singleLineHeight * strings.Length;
        }

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
                return;

            GUI.enabled = false;

            EditorGUI.LabelField(position, label);

            position.x += EditorGUIUtility.labelWidth + 2f;
            position.height = _propertyHeight;
            EditorGUI.TextArea(position, _stringToPrint);

            GUI.enabled = true;
        }
    }
}