using Game.AI;
using Game.Characters;
using Game.Inputs;
using Game.Navigations;
using UnityEngine;

namespace Game.Dev
{
    public class DevEnemyInputController : InputController<EnemyController>
    {
        [Header("Auto Link")]
        public MovementController Movement;
        public VisionController Vision;
        public NavigationController Navigation;

        private void OnFoundCollider(Collider2D cld)
        {
        }
        
        private void OnLostCollider(Collider2D cld)
        {
        }

        private void Initialize()
        {
            if (Navigation == null) Navigation = NavigationController.Instance;
            if (Movement == null) Movement = GetComponent<MovementController>();
            if (Vision == null) Vision = GetComponent<VisionController>();
            
            Navigation.SnapToGrid = true;
            
            Movement.Initialize(Character, Navigation);
            
            Vision.Initialize(Character.transform);
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            Vision.Events.Found.AddListener(OnFoundCollider);
            Vision.Events.Lost.AddListener(OnLostCollider);
        }

        private void Update()
        {
        }
    }
}