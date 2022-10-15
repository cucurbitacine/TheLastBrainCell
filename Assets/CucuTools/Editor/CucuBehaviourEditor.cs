using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CucuTools.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CucuTools.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CucuBehaviour), true, isFallback = false)]
    public class CucuBehaviourEditor : UnityEditor.Editor
    {
        private const string DefaultButtonsGroupName = "Methods";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DrawButtons(targets);
        }

        #region Drawing

        public static void DrawButtons(params Object[] targets)
        {
            var buttons = GetButtons(targets);
            
            if (buttons == null) return;

            var grouped = buttons.GroupBy(b => b.attribute.Group).OrderBy(g => g.Min(gr => gr.attribute.Order));

            foreach (var group in grouped)
            {
                DrawGroup(group.Key, group, targets);
            }
        }

        private static void DrawGroup(string groupName, IEnumerable<ButtonInfo> buttons, params Object[] targets)
        {
            groupName = string.IsNullOrEmpty(groupName) ? DefaultButtonsGroupName : groupName;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(groupName, GetHeaderGuiStyle());

            foreach (var button in buttons.OrderBy(b => b.attribute.Order))
            {
                DrawButton(button, targets);
            }
        } 
        
        private static void DrawButton(ButtonInfo button, params Object[] targets)
        {
            var attribute = button.attribute;
            var method = button.method;
                
            var buttonName = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
            var backgroundColor = GUI.backgroundColor;

            try
            {
                if (!string.IsNullOrEmpty(button.attribute.ColorHex) &&
                    ColorUtility.TryParseHtmlString(button.attribute.ColorHex, out var color))
                {
                    GUI.backgroundColor = color;
                }

                if (GUILayout.Button(buttonName, GetButtonGuiStyle()))
                {
                    foreach (var target in targets)
                    {
                        method.Invoke(target, null);                        
                    }
                }
            }
            finally
            {
                GUI.backgroundColor = backgroundColor;
            }
        }

        #endregion

        #region Getter

        private const BindingFlags methodsFilter =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public; 
        
        private static IEnumerable<MethodInfo> GetMethods(params Object[] targets)
        {
            return targets.SelectMany(t => t.GetType()
                    .GetMethods(methodsFilter)
                    .Where(m => m.GetCustomAttribute<CucuButtonAttribute>(true) != null))
                .Distinct()
                .Where(m => m.GetParameters().All(p => p.IsOptional));
        }
        
        private static IEnumerable<ButtonInfo> GetButtons(params Object[] targets)
        {
            var methods = GetMethods(targets);

            return methods.Select(m => new ButtonInfo(m));
        }

        #endregion

        #region GUIStyle

        private static GUIStyle buttonGuiStyle;
        private static GUIStyle headerGuiStyle;
        
        private static GUIStyle GetButtonGuiStyle()
        {
            if (buttonGuiStyle == null)
            {
                buttonGuiStyle = new GUIStyle(GUI.skin.button);
            }

            return buttonGuiStyle;
        }

        
        private static GUIStyle GetHeaderGuiStyle()
        {
            if (headerGuiStyle == null)
            {
                headerGuiStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperCenter
                };
            }
            return headerGuiStyle;
        }

        #endregion
        
        public class ButtonInfo
        {
            public CucuButtonAttribute attribute;
            public MethodInfo method;

            public ButtonInfo(MethodInfo method)
            {
                this.method = method;

                attribute = this.method.GetCustomAttribute<CucuButtonAttribute>(true);
            }
        }
    }
}