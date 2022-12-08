using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Tools.ScrMsg
{
    public class ScreenMessage : MonoBehaviour
    {
        [Header("Settings")]
        public float durationBase = 1f;

        [Header("References")]
        public GUISkin MsgSkin;
        public GUISkin MsgShadowSkin;
        
        private List<MsgUnit> units = new List<MsgUnit>();

        public void Message(string msg, Vector3 position, Color? color = null, float? duration = null)
        {
            units.Add(new MsgUnit()
            {
                msg = msg,
                worldPosition = position,
                screenPosition = Camera.main.WorldToScreenPoint(position),
                color = color ?? Color.yellow,
                duration = duration ?? durationBase
            });
        }
        
        private void SetupSkins()
        {
            if (MsgSkin == null) MsgSkin = Resources.Load<GUISkin>(nameof(MsgSkin));
            if (MsgShadowSkin == null) MsgShadowSkin = Resources.Load<GUISkin>(nameof(MsgShadowSkin));
        }
        
        #region MonoBehaviour

        private void OnGUI()
        {
            foreach (var unit in units)
            {
                var trg = unit.color;
                trg.a = 0f;
                GUI.color = Color.Lerp(unit.color, trg, Mathf.SmoothStep(0f, 1f, unit.lifeTime / unit.duration));
                GUI.skin = MsgShadowSkin;
                GUI.Label(new Rect(unit.screenPosition.x - 2, (Screen.height - unit.screenPosition.y) + 2, 0, 0), unit.msg);
                GUI.skin = MsgSkin;
                GUI.Label(new Rect(unit.screenPosition.x, (Screen.height - unit.screenPosition.y), 0, 0), unit.msg);
            }
        }
        
        private void Awake()
        {
            SetupSkins();
        }

        private void Update()
        {
            foreach (var unit in units)
            {
                unit.screenPosition += Vector3.up * (Time.deltaTime * 200);
                unit.lifeTime += Time.deltaTime;
            }

            for (int i = units.Count - 1; i >= 0; i--)
            {
                if (units[i].lifeTime >= units[i].duration) units.RemoveAt(i);
            }
        }

        private void OnValidate()
        {
            SetupSkins();
        }

        #endregion

        #region Static

        private static ScreenMessage Instance
        {
            get
            {
                if (instance != null) return instance;
                
                var instances = FindObjectsOfType<ScreenMessage>();
                instance = instances.FirstOrDefault();
                for (int i = 0; i < instances.Length; i++)
                {
                    if (instances[i] == instance) continue;
                    if (Application.isPlaying) Destroy(instances[i]);
                }

                if (instance != null)
                {
                    DontDestroyOnLoad(instance.gameObject);
                    return instance;
                }
                
                instance = new GameObject(nameof(ScreenMessage)).AddComponent<ScreenMessage>();
                instance.SetupSkins();
                DontDestroyOnLoad(instance.gameObject);
                
                return instance;
            }
        }

        private static ScreenMessage instance;
        
        public static void Show(string msg, Vector3 position, Color color)
        {
            if (Application.isPlaying) Instance.Message(msg, position, color);
        }

        #endregion
        
        [Serializable]
        private class MsgUnit
        {
            public string msg;

            public Vector3 worldPosition;
            public Vector3 screenPosition;
            
            public Color color;
            
            public float lifeTime;
            public float duration;
        }
    }
}