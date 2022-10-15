using System;
using System.Linq;
using UnityEngine;

namespace CucuTools.Interactables
{
    public abstract class InteractHandlerBehaviour : CucuBehaviour, ICucuContext
    {
        protected InteractHandler InteractHandler { get; private set; }
        
        private RaycastHit[] hits = new RaycastHit[16];
        private RaycastHit _raycastHit = default;
        private bool _pressedPrevious = default;

        #region SerializeField

        [Space]
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private InteractableEntity interactable = default;
        
        [Header("Cast Settings")]
        [SerializeField] private float radiusSphereCast = 0.1f;
        [SerializeField] private float maxDistanceCast = 2f;
        [SerializeField] private LayerMask layerMaskCast = 1;
        [SerializeField] private QueryTriggerInteraction interactionCast = QueryTriggerInteraction.Ignore;
        
        [Header("Other Settings")]
        [SerializeField] private Transform[] ignoreTransforms = default;
        
        #endregion

        #region Properties

        public Transform[] IgnoreTransforms => ignoreTransforms ?? (ignoreTransforms = Array.Empty<Transform>());
        
        public bool Clicked { get; private set; }
        
        public InteractableEntity Interactable
        {
            get => interactable;
            private set => interactable = value;
        }

        public RaycastHit RaycastHit
        {
            get => _raycastHit;
            private set
            {
                _raycastHit = value;
                Interactable = _raycastHit.transform != null
                    ? _raycastHit.transform.GetComponent<InteractableEntity>()
                    : null;
            }
        }

        #endregion

        #region Virtual Properties

        public virtual bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }
        
        public virtual float RadiusSphereCast
        {
            get => radiusSphereCast;
            set => radiusSphereCast = value;
        }

        public virtual float MaxDistanceCast
        {
            get => maxDistanceCast;
            set => maxDistanceCast = value;
        }

        public virtual LayerMask LayerMaskCast
        {
            get => layerMaskCast;
            set => layerMaskCast = value;
        }

        public virtual QueryTriggerInteraction InteractionCast
        {
            get => interactionCast;
            set => interactionCast = value;
        }

        #endregion

        #region Abstract Properties

        public abstract bool Pressed { get; protected set; }
        public abstract Vector3 OriginCast { get; }
        public abstract Vector3 DirectionCast { get; }

        #endregion
        
        #region Virtual MonoBehaviour

        protected virtual void Awake()
        {
            InteractHandler = new InteractHandler();
            Interactable = null;
        }

        protected virtual void LateUpdate()
        {
            Clicked = !_pressedPrevious && Pressed;
            
            InteractHandler.Update(this, Pressed, Interactable);
            
            _pressedPrevious = Pressed;
        }

        protected virtual void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(OriginCast, RadiusSphereCast, DirectionCast, hits, MaxDistanceCast, LayerMaskCast, InteractionCast);

            RaycastHit = default;
            
            if (IsEnabled)
            {
                RaycastHit bestHit = default;
                var distance = float.PositiveInfinity;
                
                for (var i = 0; i < count; i++)
                {
                    var hit = hits[i];

                    if (IgnoreTransforms.Any(ig => hit.transform.IsChildOf(ig))) continue;
                    
                    if (hit.distance < distance)
                    {
                        bestHit = hit;
                        distance = hit.distance;
                    }
                }

                RaycastHit = bestHit;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (IsEnabled)
            {
                Gizmos.color = RaycastHit.transform != null ? Color.yellow : Color.red;
                Gizmos.color = Pressed ? Color.green : Gizmos.color;
            }
            else
            {
                Gizmos.color = Color.grey;
            }

            Gizmos.DrawWireSphere(OriginCast, RadiusSphereCast);
            
            if (RaycastHit.transform != null)
            {
                Gizmos.DrawWireSphere(RaycastHit.point + RaycastHit.normal * RadiusSphereCast, RadiusSphereCast);
                Gizmos.DrawLine(OriginCast, RaycastHit.point + RaycastHit.normal * RadiusSphereCast);
            }
            else
            {
                Gizmos.DrawWireSphere(OriginCast + DirectionCast * MaxDistanceCast, RadiusSphereCast);
                Gizmos.DrawLine(OriginCast, OriginCast + DirectionCast * MaxDistanceCast);
            }
        }

        #endregion
    }
}
