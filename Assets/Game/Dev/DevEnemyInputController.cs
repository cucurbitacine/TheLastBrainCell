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
        public DetectionController Detection;
        public NavigationController Navigation;

        public float periodUpdatePath = 0.2f;
        private float _timer;
        
        private Vector2 _initPos;
        private Vector2 _initRot;
        
        private PlayerController _player;
        
        private void OnFoundCollider(DetectionSample sample)
        {
            if (sample.status != DetectionStatus.Detected) return;

            var cld = sample.collider;

            if (cld == null) return;
            
            _player = cld.GetComponent<PlayerController>();
        }
        
        private void OnLostCollider(DetectionSample sample)
        {
            if (sample.status != DetectionStatus.Undefined) return;

            var cld = sample.collider;

            if (cld == null) return;
            
            if (_player != cld.GetComponent<PlayerController>()) return;

            _player = null;
            
            if (Navigation.TryFindPath(Character.position, _initPos, out var path))
            {
                Movement.FollowThePath(path, () => Character.View(_initRot));
            }
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

            _initPos = Character.position;
            _initRot = Character.direction;
        }

        private void Start()
        {
            Detection.OnStatusChanged.AddListener(OnFoundCollider);
            Detection.OnStatusChanged.AddListener(OnLostCollider);
        }
        
        private void Update()
        {
            if (_player == null) return;

            if (_timer <= 0f)
            {
                if (Navigation.TryFindPath(Character.position, _player.position, out var path))
                {
                    Movement.FollowThePath(path);
                }
            }

            _timer += Time.deltaTime;

            if (_timer >= periodUpdatePath) _timer = 0f;
        }
    }
}