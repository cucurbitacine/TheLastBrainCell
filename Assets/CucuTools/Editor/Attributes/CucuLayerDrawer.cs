using CucuTools.Attributes;
using UnityEditor;
using UnityEngine;

namespace CucuTools.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(CucuLayerAttribute))]
    public class CucuLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            property.intValue = EditorGUI.LayerField(position, property.intValue);

            EditorGUI.EndProperty();
        }
    }
}