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

        [Space] public CinemachineVirtualCamera virtualCamera;

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
            if (virtualCamera == null)
            {
                var activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
                if (activeVirtualCamera is CinemachineVirtualCamera cam)
                {
                    virtualCamera = cam;
                }

                if (virtualCamera == null) yield break;
            }

            var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = intensity;

            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0f;
        }
    }
}