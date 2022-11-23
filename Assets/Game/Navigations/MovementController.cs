using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CharacterController = Game.Characters.CharacterController;

namespace Game.Navigations
{
    [DisallowMultipleComponent]
    public class MovementController : MonoBehaviour
    {
        [Range(0.001f, 0.1f)]
        [SerializeField] private float toleranceDistance = 0.01f;
        [Min(0f)]
        [SerializeField] private float timeLimit = 20f;
        
        [Space]
        [SerializeField] private CharacterController character;
        [SerializeField] private NavigationController navigation;
        
        public float ToleranceDistance
        {
            get => toleranceDistance;
            set => toleranceDistance = value;
        }

        public float TimeLimit
        {
            get => timeLimit;
            set => timeLimit = value;
        }

        public CharacterController Character => character ??= GetComponent<CharacterController>();

        public NavigationController Navigation => navigation ??= NavigationController.Instance;

        private Coroutine _followToPointsProcess = null;

        public void Initialize(CharacterController character, NavigationController navigation)
        {
            this.character = character;
            this.navigation = navigation;
        }
        
        public void Initialize(CharacterController character)
        {
            Initialize(character, NavigationController.Instance);
        }

        public void Deinitialize()
        {
            this.character = null;
            this.navigation = null;
        }
        
        public void Stop()
        {
            if (_followToPointsProcess != null) StopCoroutine(_followToPointsProcess);
            
            Character.Stop();
        }

        public bool TryFollowToPoint(Vector2 point, Action action = null)
        {
            if (Navigation == null) return false;

            if (!Navigation.TryFindPath(Character.position, point, out var path)) return false;
            
            FollowThePath(path, action);

            return true;
        }
        
        public void FollowThePath(List<Vector2> path, Action action = null)
        {
            Stop();

            _followToPointsProcess = StartCoroutine(FollowThePathProcess(path, action));
        }
        
        public void FollowThePath(params Vector2[] path)
        {
            FollowThePath(path.ToList());
        }

        private IEnumerator FollowThePathProcess(List<Vector2> path, Action action)
        {
            for (var i = 0; i < path.Count; i++)
            {
                var targetPoint = path[i];

                if (i > 0)
                {
                    if (Vector2.Distance(targetPoint, path[i - 1]) <= 0.001f) continue;
                }
                
                var startPoint = Character.position;
                var timer = 0f;

                while (true)
                {
                    var vector = targetPoint - Character.position;

                    if (NeedStop(startPoint, targetPoint, Character.position, ToleranceDistance, timer, TimeLimit))
                    {
                        break;
                    }

                    Character.Move(vector);
                    Character.View(vector);

                    yield return new WaitForFixedUpdate();
                    timer += Time.fixedDeltaTime;
                }
            }

            Character.Stop();
            Character.View(Character.direction);
            
            action?.Invoke();
        }
        
        private static bool NeedStop(
            Vector2 startPoint, Vector2 targetPoint,
            Vector2 currentPoint, float minDistance,
            float duration, float maxDuration)
        {
            var direction = targetPoint - startPoint;
            var vector = targetPoint - currentPoint;

            return Vector2.Distance(currentPoint, targetPoint) <= minDistance ||
                   Vector2.Dot(direction, vector) <= 0 ||
                   maxDuration <= duration;
        }
    }
}