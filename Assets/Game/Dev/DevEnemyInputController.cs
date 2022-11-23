using System;
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
        [Space]
        [Min(0f)]
        public float periodUpdatePath = 0.1f;
        public bool turnOnMoveByMouse = false;
        
        [Space]
        public PlayerController Player;
        public bool VisiblePlayer;
        public Vector2 LastPlayerPoint;

        [Space]
        public DetectionController Detection;
        public MovementController Movement;
        
        private List<Vector2> _currentPath;
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

                        Move(_initPos, () => Character.View(_initDir));
                    }
                    else if (sample.status == DetectionStatus.Losing)
                    {
                        VisiblePlayer = false;

                        LastPlayerPoint = Player.position;

                        Move(LastPlayerPoint);
                    }
                    else if (sample.status == DetectionStatus.Detected)
                    {
                        VisiblePlayer = true;
                    }
                }
            }
        }

        private void Move(Vector2 point, Action actionAfter = null)
        {
            if (!Movement.gameObject.activeInHierarchy) return;
            
            if (Movement.Navigation.TryFindPath(Character.position, point, out _currentPath))
            {
                Movement.FollowThePath(_currentPath, actionAfter);
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

                Move(worldPoint);
            }

            if (Player != null)
            {
                if (_timer <= 0f)
                {
                    var targetPoint = VisiblePlayer ? Player.position : LastPlayerPoint;
                    
                    Move(targetPoint);
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