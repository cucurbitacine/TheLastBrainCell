using System;
using CucuTools.Attributes;
using UnityEditor;
using UnityEngine;

namespace CucuTools.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(CucuSceneAttribute))]
    public class CucuSceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Only string fields");
                return;
            }
    
            var sceneObject = GetSceneObject(property.stringValue);
            var scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);
            if (scene == null)
            {
                property.stringValue = "";
            }
            else if (scene.name != property.stringValue)
            {
                var sceneObj = GetSceneObject(scene.name);
                if (sceneObj == null)
                {
                    Debug.LogWarning("The scene " + scene.name +
                                     " cannot be used. To use this scene add it to the build settings for the project");
                }
                else
                {
                    property.stringValue = scene.name;
                }
            }
        }
    
        protected SceneAsset GetSceneObject(string sceneObjectName)
        {
            if (string.IsNullOrEmpty(sceneObjectName)) return null;
    
            foreach (var editorScene in EditorBuildSettings.scenes)
            {
                if (editorScene.path.IndexOf(sceneObjectName, StringComparison.Ordinal) != -1)
                {
                    return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
                }
            }
    
            Debug.LogWarning("Scene [" + sceneObjectName +
                             "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
            return null;
        }
    }
}