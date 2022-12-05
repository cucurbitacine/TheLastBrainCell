using System;
using Game.Characters.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Dev.Inputs
{
    public class VolumeTest : MonoBehaviour
    {
        public Vector2 worldPosition;
        public Vector2 screenPoint;
        public Vector2 viewportPoint;
        
        
        public PlayerController player;

        public VolumeProfile profile;
        public LensDistortion lensDistortion;
        public Camera MainCamera => Camera.main;

        private Vector2 _initCenter;
        
        private void OnEnable()
        {
            if (profile.TryGet(out lensDistortion))
            {
                _initCenter = lensDistortion.center.value;
            }
        }

        private void OnDisable()
        {
            if (lensDistortion != null) lensDistortion.center.value = _initCenter;
        }

        private void Update()
        {
            worldPosition = player.position;
            screenPoint = MainCamera.WorldToScreenPoint(worldPosition);
            viewportPoint = MainCamera.ScreenToViewportPoint(screenPoint);

            if (lensDistortion != null)
            {
                lensDistortion.center.value = viewportPoint;
            }
        }
    }
}
