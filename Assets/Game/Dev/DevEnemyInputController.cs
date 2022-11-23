using System.Collections.Generic;
using Game.AI;
using Game.Characters;
using Game.Inputs;
using Game.Navigations;
using UnityEngine;

namespace Game.Dev
{
    public class DevEnemyInputController : InputController<EnemyController>
    {
        [Min(0f)]
        public float periodUpdatePath = 0.1f;
        public PlayerController Player;
        public bool VisiblePlayer;
        public Vector2 LastPlayerPoint;
        
        [Space]
        public bool turnOnMoveByMouse = false;
        
        [Space]
        public DetectionController Detection;
        public MovementController Movement;
        
        public List<Vector2> _currentPath;
        private Camera _camera;
        private Vector2 _initPos;
        private Vector2 _initDir;
        private float _timer;
        
        private void DetectionChange(DetectionSample sample)
        {
            if (Player == null)
            {
                if (sample.status == DetectionStatus.Detected)
                {
                    var player = sample.collider.GetComponent<PlayerController>();

                    if (player != null)
                    {
                        VisiblePlayer = true;
                        Player = player;
                        _timer = 0f;
                    }
                }
            }
            else
            {
                var player = sample.collider.GetComponent<PlayerController>();

                if (player == Player)
                {
                    if (sample.status == DetectionStatus.Undefined)
                    {
                        Player = null;
                        _timer = 0f;

                        if (Movement.Navigation.TryFindPath(Character.position, _initPos, out _currentPath))
                        {
                            Movement.FollowThePath(_currentPath, () => Character.View(_initDir));
                        }
                    }
                    else if (sample.status == DetectionStatus.Losing)
                    {
                        VisiblePlayer = false;

                        LastPlayerPoint = Player.position;
                        
                        if (Movement.Navigation.TryFindPath(Character.position, LastPlayerPoint, out _currentPath))
                        {
                            Movement.FollowThePath(_currentPath);
                        }
                    }
                    else if (sample.status == DetectionStatus.Detected)
                    {
                        VisiblePlayer = true;
                    }
                }
            }
        }

        private void Awake()
        {
            _initPos = Character.position;
            _initDir = Character.direction;
        }

        private void OnEnable()
        {
            Detection.OnStatusChanged.AddListener(DetectionChange);
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (turnOnMoveByMouse && Input.GetKeyDown(KeyCode.Mouse0))
            {
                var worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);

                if (Movement.Navigation.TryFindPath(Character.position, worldPoint, out _currentPath))
                {
                    Movement.FollowThePath(_currentPath);
                }
            }

            if (Player != null)
            {
                if (_timer <= 0f)
                {
                    var targetPoint = VisiblePlayer ? Player.position : LastPlayerPoint;
                    
                    if (Movement.Navigation.TryFindPath(Character.position, targetPoint, out _currentPath))
                    {
                        Movement.FollowThePath(_currentPath);
                    }
                }

                if (VisiblePlayer) _timer += Time.deltaTime;

                if (periodUpdatePath <= _timer)
                {
                    _timer = 0f;
                }
            }
        }

        private void OnDisable()
        {
            Detection.OnStatusChanged.RemoveListener(DetectionChange);
        }

        private void OnDrawGizmos()
        {
            if (_currentPath != null && _currentPath.Count > 0)
            {
                for (var i = 0; i < _currentPath.Count; i++)
                {
                    Gizmos.DrawWireSphere(_currentPath[i], 0.1f);

                    if (i > 0)
                    {
                        Gizmos.DrawLine(_currentPath[i - 1], _currentPath[i]);
                    }
                }
            }
        }
    }
}