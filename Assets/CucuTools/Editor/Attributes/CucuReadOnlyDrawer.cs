using CucuTools.Attributes;
using UnityEditor;
using UnityEngine;

namespace CucuTools.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(CucuReadOnlyAttribute))]
    public class CucuReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            string labelStr = $"{label.text} (read only)";
 
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    EditorGUI.IntField(position, labelStr, prop.intValue);
                    break;
                case SerializedPropertyType.Boolean:
                    EditorGUI.Toggle(position, labelStr, prop.boolValue);
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.FloatField(position, labelStr, prop.floatValue);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.TextField(position, labelStr, prop.stringValue);
                    break;
                case SerializedPropertyType.Color:
                    EditorGUI.ColorField(position, labelStr, prop.colorValue);
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.Vector2Field(position, labelStr, prop.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.Vector3Field(position, labelStr, prop.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.Vector4Field(position, labelStr, prop.vector4Value);
                    break;
                case SerializedPropertyType.Vector2Int:
                    EditorGUI.Vector2IntField(position, labelStr, prop.vector2IntValue);
                    break;
                case SerializedPropertyType.Vector3Int:
                    EditorGUI.Vector3IntField(position, labelStr, prop.vector3IntValue);
                    break;
                default:
                    EditorGUI.LabelField(position, labelStr, "<not supported>");
                    break;
            }
        }
    }
}