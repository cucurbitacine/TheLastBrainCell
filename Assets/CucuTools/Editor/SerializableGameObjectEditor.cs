using System;
using System.Collections.Generic;
using System.Linq;
using CucuTools.Serialization;
using CucuTools.Serialization.Impl;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CucuTools.Editor
{
    [CustomEditor(typeof(SerializableGameObject))]
    public class SerializableGameObjectEditor : UnityEditor.Editor
    {
        private Vector2 scrollView;
        private bool showedList = true;
        
        public override void OnInspectorGUI()
        {
            var serializable = (SerializableGameObject)target;
            serializable.UpdateComponents();

            ShowTarget(serializable);
                
            GUILayout.Space(8);
            
            if (GUILayout.Button(showedList ? "Hide" : "Serializable Components"))
            {
                showedList = !showedList;
            }
            
            GUILayout.Space(8);
            
            if (showedList) ShowComponents(serializable.SerializableComponents.ToArray());
        }

        private void ShowTarget(SerializableGameObject entity)
        {
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.Box("Reference: ");
            entity.GameObjectRef = (GameObject)EditorGUILayout.ObjectField(entity.GameObjectRef, typeof(GameObject), true);
            
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.Label(entity.Cuid?.Guid.ToString() ?? "Without cuid");
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
        
        private void ShowComponents(params SerializableComponent[] components)
        {
            scrollView = GUILayout.BeginScrollView(scrollView);
            
            foreach (var component in components)
                ShowComponent(component);

            GUILayout.EndScrollView();
        }

        private void ShowComponent(SerializableComponent component)
        {
            GUILayout.BeginHorizontal();

            component.IsEnabled = GUILayout.Toggle(component.IsEnabled, component.ComponentType.Name);
            EditorGUILayout.ObjectField(component.ComponentRef, component.ComponentType, true);

            GUILayout.EndHorizontal();
        }

        public const string MenuItem = Cucu.GameObject + Cucu.Serialization + Serialize; 
        public const string Serialize = "Serialize GameObject"; 
        
        [MenuItem(MenuItem, false, 10)]
        public static void SerializeGameObject(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject;

            if (target == null) return;
            
            var cuid = CucuIdentity.GetOrAdd(target);
            var go = new GameObject($"Serializable <{target.name}>").AddComponent<SerializableGameObject>();
            go.GameObjectRef = target;

            go.gameObject.AddComponent<SerializableTransform>().GameObjectRef = go.GameObjectRef;

            if (target.GetComponent<Rigidbody>() != null)
                go.gameObject.AddComponent<SerializableRigidbody>().GameObjectRef = go.GameObjectRef;
            
            Undo.RegisterCreatedObjectUndo(go.gameObject, "Create " + go.name);

            Selection.activeObject = go;
        }
        
        [MenuItem(MenuItem, true)]
        public static bool ValidateSerializeGameObject()
        {
            return Selection.activeGameObject != null;
        }
    }
}