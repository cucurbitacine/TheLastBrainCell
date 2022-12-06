using UnityEngine;

namespace Game.Environment.Parallax
{
    public class ParallaxBackground : MonoBehaviour
    {
        public float parallaxEffectMultiplier = 1f;
        public bool followCameraMain = true;
        
        [Space]
        public Transform followTransform;
        public Transform parallaxTransform;

        private Vector3 _lastFollowPosition;

        private Camera CameraMain => Camera.main;
        private Transform CameraTransform => CameraMain.transform;
        
        private void Awake()
        {
            if (followCameraMain) followTransform = CameraTransform;
            if (parallaxTransform == null) parallaxTransform = transform;
        }

        private void Start()
        {
            _lastFollowPosition = followTransform.position;
        }

        private void LateUpdate()
        {
            var followPosition = followTransform.position;
            var deltaMovement = followPosition - _lastFollowPosition;
            parallaxTransform.position += deltaMovement * parallaxEffectMultiplier;
            _lastFollowPosition = followPosition;
        }
    }
}
