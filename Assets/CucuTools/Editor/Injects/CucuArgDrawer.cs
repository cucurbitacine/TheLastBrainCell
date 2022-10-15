using CucuTools.Injects;
using UnityEditor;
using UnityEngine;

namespace CucuTools.Editor.Injects
{
    [CustomPropertyDrawer(typeof(CucuArgAttribute))]
    public class CucuArgDrawer : PropertyDrawer
    {
        private const float scale = 0.1f;

        private static readonly float height =
            EditorGUI.GetPropertyHeight(SerializedPropertyType.Character, new GUIContent(""));

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var shift = scale * position.width;

            var pos = new Rect(position) {width = shift * 0.75f, height = height};
            var isDefault = property.FindPropertyRelative("isDefault").boolValue;
            EditorGUI.HelpBox(pos, "", isDefault ? MessageType.Warning : MessageType.Info);

            position.x += shift;
            position.width -= shift;

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}