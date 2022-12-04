using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class CameraShaker : MonoBehaviour
    {
        public bool isActive = true;
        public float intensityDefault = 1f;
        public float durationDefault = 0.2f;

        [Space]
        public NoiseSettings noiseProfile;
        
        private CinemachineBrain _brainCamera;

        public CinemachineVirtualCamera VirtualCamera => (CinemachineVirtualCamera)_brainCamera.ActiveVirtualCamera;

        private Coroutine _shaking;

        public void Shake()
        {
            Shake(intensityDefault, durationDefault);
        }

        public void Shake(float intensity, float duration)
        {
            if (!isActive) return;

            if (_shaking != null) StopCoroutine(_shaking);
            _shaking = StartCoroutine(Shaking(intensity, duration));
        }

        private IEnumerator Shaking(float intensity, float duration)
        {
            var noise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (noise == null)
            {
                noise = VirtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_NoiseProfile = noiseProfile;
            }
            
            noise.m_AmplitudeGain = intensity;
            noise.m_FrequencyGain = 0.5f;
            
            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0f;
        }

        private void Awake()
        {
            _brainCamera = FindObjectOfType<CinemachineBrain>();
        }
    }
}