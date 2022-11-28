using Game.AI;
using Game.Characters;
using UnityEngine;

namespace Game.Dev.Detection
{
    public class DetectionStatusInfo : MonoBehaviour
    {
        public SpriteRenderer sprite;
        public DetectionController detection;
        public CharacterControllerBase character;  
            
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

        private void OnCharacterDead()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
            
            sprite.color = undefined;
        }
        
        private void OnEnable()
        {
            detection.OnStatusChanged.AddListener(OnDetectionChanged);
            character.Health.Events.OnValueIsEmpty.AddListener(OnCharacterDead);
        }

        private void Start()
        {
            sprite.color = undefined;
        }

        private void OnDisable()
        {
            detection.OnStatusChanged.RemoveListener(OnDetectionChanged);
            character.Health.Events.OnValueIsEmpty.RemoveListener(OnCharacterDead);
        }
    }
}
