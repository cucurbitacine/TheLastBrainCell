using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.AI
{
    public class DetectionController : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField] private float detectionDelay = 1f;
        [Min(0f)]
        [SerializeField] private float losingDelay = 1f;
        
        [Space]
        [SerializeField] private UnityEvent<DetectionSample> onStatusChanged = null;
        
        [Space]
        [SerializeField] private VisionController vision;

        private readonly List<DetectionSample> _detectableSamples = new List<DetectionSample>();
        private readonly Dictionary<DetectionSample, float> _sampleTimes = new Dictionary<DetectionSample, float>();

        public float DetectionDelay
        {
            get => detectionDelay;
            set => detectionDelay = Mathf.Max(0f, value);
        }

        public float LosingDelay
        {
            get => losingDelay;
            set => losingDelay = Mathf.Max(0f, value);
        }

        public VisionController Vision => vision;

        public UnityEvent<DetectionSample> OnStatusChanged => onStatusChanged ??= new UnityEvent<DetectionSample>();

        #region Private API

        private void StartDetecting(DetectionSample sample)
        {
            sample.previous = sample.status;
            sample.status = DetectionStatus.Detecting;

            _detectableSamples.Add(sample);
            _sampleTimes.Add(sample, 0f);
            
            OnStatusChanged.Invoke(sample);
        }
        
        private void Detected(DetectionSample sample)
        {
            sample.previous = sample.status;
            sample.status = DetectionStatus.Detected;

            _sampleTimes[sample] = 0f;
                    
            OnStatusChanged.Invoke(sample);
        }

        private void StartLosing(DetectionSample sample)
        {
            sample.previous = sample.status;
            sample.status = DetectionStatus.Losing;
            
            _sampleTimes[sample] = 0f;
                    
            OnStatusChanged.Invoke(sample);
        }
        
        private void Lost(DetectionSample sample)
        {
            sample.previous = sample.status;
            sample.status = DetectionStatus.Undefined;
            
            _detectableSamples.Remove(sample);
            _sampleTimes.Remove(sample);
            
            OnStatusChanged.Invoke(sample);
        }
        
        private void OnFoundCollider(Collider2D cld)
        {
            var sample = _detectableSamples.FirstOrDefault(ds => ds.collider == cld);
            
            if (sample != null && sample.status == DetectionStatus.Losing)
            {
                Detected(sample);
            }
            else
            {
                StartDetecting(new DetectionSample(cld));
            }
        }
        
        private void OnLostCollider(Collider2D cld)
        {
            var sample = _detectableSamples.FirstOrDefault(ds => ds.collider == cld);

            if (sample != null)
            {
                if (sample.status == DetectionStatus.Detected)
                {
                    StartLosing(sample);
                }
                else if (sample.status == DetectionStatus.Detecting)
                {
                    Lost(sample);
                }
            }
        }
        
        private void UpdateDetection(float deltaTime)
        {
            var removed = new List<DetectionSample>();
            
            for (var i = 0; i < _detectableSamples.Count; i++)
            {
                var sample = _detectableSamples[i];

                _sampleTimes[sample] += deltaTime;

                if (sample.collider == null)
                {
                    removed.Add(sample);
                }
                else
                {
                    if (sample.status == DetectionStatus.Detecting)
                    {
                        if (_sampleTimes[sample] >= DetectionDelay)
                        {
                            Detected(sample);
                        }
                    }
                    else if (sample.status == DetectionStatus.Losing)
                    {
                        if (_sampleTimes[sample] >= LosingDelay)
                        {
                            removed.Add(sample);
                        }
                    }
                }
            }

            foreach (var sample in removed)
            {
                Lost(sample);
            }
        }

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            Vision.Events.Found.AddListener(OnFoundCollider);
            Vision.Events.Lost.AddListener(OnLostCollider);
        }

        private void Update()
        {
            UpdateDetection(Time.deltaTime);
        }

        private void OnDisable()
        {
            Vision.Events.Found.RemoveListener(OnFoundCollider);
            Vision.Events.Lost.RemoveListener(OnLostCollider);
        }

        private void OnDrawGizmos()
        {
            for (var i = 0; i < _detectableSamples.Count; i++)
            {
                var ds = _detectableSamples[i];
                if (ds.collider == null) continue;

                var clr = Color.cyan;
                if(ds.status == DetectionStatus.Detected) clr = Color.green;
                if (ds.status == DetectionStatus.Losing) clr = Color.yellow;
                Gizmos.color = clr;

                var pos = ds.collider.transform.position;
                Gizmos.DrawLine(Vision.position, pos);
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }

        #endregion
    }

    [Serializable]
    public class DetectionSample
    {
        public Collider2D collider;
        public DetectionStatus status;
        public DetectionStatus previous;

        public DetectionSample(Collider2D cld)
        {
            collider = cld;
            previous = DetectionStatus.Undefined;
            status = DetectionStatus.Detecting;
        }
    }

    public enum DetectionStatus
    {
        Undefined,
        Detecting,
        Detected,
        Losing,
    }
}