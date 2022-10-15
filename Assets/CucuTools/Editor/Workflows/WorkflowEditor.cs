using System;
using System.Collections.Generic;
using System.Linq;
using CucuTools.Workflows;
using CucuTools.Workflows.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CucuTools.Editor.Workflows
{
    public class WorkflowEditor : EditorWindow
    {
        public const string AddComponent = Cucu.AddComponent;

        [MenuItem(Cucu.CreateGameObject + WorkflowBehaviour.ObjectName)]
        public static void CreateWorkflow()
        {
            Selection.objects = new Object[]
            {
                new GameObject(WorkflowBehaviour.ObjectName).AddComponent<WorkflowBehaviour>().gameObject
            };
        }

        [MenuItem(Cucu.Tools + Cucu.WorkflowGroup + "Workflow viewer")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow(typeof(WorkflowEditor), false, "Viewer").Show();
        }

        private WorkflowBehaviour workflow;

        private static Dictionary<StateEntity, List<StateEntity>> states =
            new Dictionary<StateEntity, List<StateEntity>>();

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            states.Clear();

            if (workflow == null) workflow = FindObjectOfType<WorkflowBehaviour>();

            workflow = (WorkflowBehaviour) EditorGUILayout.ObjectField(workflow, typeof(WorkflowBehaviour), true);

            if (workflow != null)
            {
                ShowState(null, workflow, "");
            }
        }

        private bool CanShow(StateEntity from, StateEntity target)
        {
            if (from == null) return true;

            if (!states.TryGetValue(from, out var list))
            {
                list = new List<StateEntity>();
                states.Add(from, list);
            }

            if (list.Contains(target)) return false;

            list.Add(target);

            return true;
        }

        private void ShowState(StateEntity from, StateEntity target, string prefix)
        {
            if (!CanShow(from, target)) return;


            if (from != null)
            {
                var style = new GUIStyle(GUI.skin.box);

                if (from.IsPlaying)
                    style.normal.textColor = Color.cyan;  
                
                GUILayout.Box(prefix + from.gameObject.name + " -> " + target.gameObject.name, style);
            }
            
            var transitions = target.gameObject
                .GetComponentsInChildren<TransitionEntity>()
                .Where(t => t.Owner == target);

            foreach (var trans in transitions)
            {
                if (trans is SwitchEntity switcher)
                {
                    foreach (var t in switcher.Targets)
                    {
                        if (t != null) ShowState(target, t, prefix + "\t");
                    }
                }

                if (trans is TransitionBehaviour tr)
                {
                    if (tr.Target != null) ShowState(target, tr.Target, prefix + "\t");
                }
            }

            if (target is WorkflowBehaviour w)
            {
                if (w.First != null) ShowState(w, w.First, prefix + "\t");
            }
        }
    }
}