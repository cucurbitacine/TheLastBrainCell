using System.Collections;
using Game.Characters.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Dev.Inputs
{
    public class PostProcessingEffects : MonoBehaviour
    {
        [Header("Hit effect")]
        public float durationHit = 0.3f;
        public Color colorHit = Color.red;
        [Range(0f, 1f)]
        public float intensityHit = 1f;
        
        [Header("Info")]
        public Vector2 worldPosition;
        public Vector2 screenPoint;
        public Vector2 viewportPoint;
        
        [Header("References")]
        public PlayerController player;
        public VolumeProfile profile;

        private Coroutine _hitting = null;
        private Vignette _vignette;
        private Color _initColor;
        private float _initIntensity;
        
        private LensDistortion _lensDistortion;
        private Vector2 _initCenter;

        public Camera MainCamera => Camera.main;
        
        private void Hit()
        {
            if (_hitting != null) StopCoroutine(_hitting);
            _hitting = StartCoroutine(Hitting());
        }

        private IEnumerator Hitting()
        {
            _vignette.color.value = colorHit;
            _vignette.intensity.value = intensityHit;

            yield return new WaitForSeconds(durationHit);
            
            ResetVignette();
        }

        private void ResetVignette()
        {
            if (_vignette == null) return;
            
            _vignette.color.value = _initColor;
            _vignette.intensity.value = _initIntensity;
        }
        
        private void OnHealthChanged(int diff)
        {
            if (diff < 0) Hit();
        }
        
        private void OnEnable()
        {
            if (profile.TryGet(out _lensDistortion))
            {
                _initCenter = _lensDistortion.center.value;
            }
            
            if (profile.TryGet(out _vignette))
            {
                _initColor = _vignette.color.value;
                _initIntensity = _vignette.intensity.value;
            }
            
            player.Health.Events.OnValueChanged.AddListener(OnHealthChanged);
        }

        private void OnDisable()
        {
            if (_lensDistortion != null) _lensDistortion.center.value = _initCenter;
            ResetVignette();
            
            player.Health.Events.OnValueChanged.RemoveListener(OnHealthChanged);
        }

        private void Update()
        {
            if (player == null) return;
            
            worldPosition = player.position;
            screenPoint = MainCamera.WorldToScreenPoint(worldPosition);
            viewportPoint = MainCamera.ScreenToViewportPoint(screenPoint);

            if (_lensDistortion != null)
            {
                _lensDistortion.center.value = viewportPoint;
            }
        }
    }
}
