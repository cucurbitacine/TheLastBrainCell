using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = Game.Characters.CharacterController;

namespace Game.Navigations
{
    [DisallowMultipleComponent]
    public class MovementController : MonoBehaviour
    {
        [Range(0.001f, 0.1f)]
        [SerializeField] private float toleranceMovement = 0.01f;
        [Space]
        [SerializeField] private CharacterController character;
        [SerializeField] private NavigationController navigation;
        
        public float ToleranceMovement
        {
            get => toleranceMovement;
            set => toleranceMovement = value;
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
        
        public void FollowThePath(IEnumerable<Vector2> path, Action action = null)
        {
            Stop();

            _followToPointsProcess = StartCoroutine(FollowThePathProcess(path, action));
        }
        
        public void FollowThePath(params Vector2[] path)
        {
            FollowThePath(path as IEnumerable<Vector2>);
        }

        private IEnumerator FollowThePathProcess(IEnumerable<Vector2> path, Action action)
        {
            var lastPoint = Vector2.zero;
            
            foreach (var point in path)
            {
                if (Vector2.Distance(lastPoint, point) <= 0.001f) continue;
                
                var nextPoint = false;
                var prevVector = point - Character.position;
                
                Character.Move(prevVector);
                Character.View(prevVector);
                
                while (!nextPoint)
                {
                    var vector = point - Character.position;
                    
                    nextPoint = vector.magnitude <= ToleranceMovement ||
                                prevVector.sqrMagnitude < vector.sqrMagnitude ||
                                prevVector.normalized == -vector.normalized;

                    prevVector = vector;

                    yield return new WaitForFixedUpdate();
                }

                lastPoint = point;
            }

            Character.Stop();
            
            action?.Invoke();
        }
    }
}