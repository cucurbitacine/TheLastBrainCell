using Game.AI;
using UnityEngine;

namespace Game.Dev.Detection
{
    public class DetectionStatusInfo : MonoBehaviour
    {
        public SpriteRenderer sprite;
        public DetectionController detection;
        
        public Color undefined = Color.black;
        public Color detecting = Color.cyan;
        public Color detected = Color.red;
        public Color losing = Color.yellow;

        private void OnDetectionChanged(DetectionSample sample)
        {
            if (sample.status == DetectionStatus.Undefined) sprite.color = undefined;
            if (sample.status == DetectionStatus.Detecting) sprite.color = detecting;
            if (sample.status == DetectionStatus.Detected) sprite.color = detected;
            if (sample.status == DetectionStatus.Losing) sprite.color = losing;
        }
        
        private void OnEnable()
        {
            detection.OnStatusChanged.AddListener(OnDetectionChanged);
        }

        private void Start()
        {
            sprite.color = undefined;
        }

        private void OnDisable()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
        }
    }
}
