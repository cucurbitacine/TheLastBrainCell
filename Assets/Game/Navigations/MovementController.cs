using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Characters;
using UnityEngine;

namespace Game.Navigations
{
    /// <summary>
    /// Movement logic which using <see cref="NavigationController"/> for getting path
    /// </summary>
    [DisallowMultipleComponent]
    public class MovementController : MonoBehaviour
    {
        [Range(0.001f, 0.1f)]
        [SerializeField] private float toleranceDistance = 0.01f;
        [Min(0f)]
        [SerializeField] private float timeLimit = 20f;
        
        [Space]
        [SerializeField] private CharacterControllerBase character;
        [SerializeField] private NavigationController navigation;

        #region Public API

        /// <summary>
        /// Minimum distance to destination point
        /// </summary>
        public float ToleranceDistance
        {
            get => toleranceDistance;
            set => toleranceDistance = value;
        }

        /// <summary>
        /// Maximum time limit to moving to the one point
        /// </summary>
        public float TimeLimit
        {
            get => timeLimit;
            set => timeLimit = value;
        }

        /// <summary>
        /// Movable character
        /// </summary>
        public CharacterControllerBase Character => character ??= GetComponent<CharacterControllerBase>();

        /// <summary>
        /// Using navigation
        /// </summary>
        public NavigationController Navigation => navigation ??= NavigationController.Instance;

        private Coroutine _followToPointsProcess = null;

        /// <summary>
        /// Setup movement controller
        /// </summary>
        /// <param name="character"></param>
        /// <param name="navigation"></param>
        public void Initialize(CharacterControllerBase character, NavigationController navigation)
        {
            this.character = character;
            this.navigation = navigation;
        }
        
        /// <summary>
        /// Setup movement controller
        /// </summary>
        /// <param name="character"></param>
        public void Initialize(CharacterControllerBase character)
        {
            Initialize(character, NavigationController.Instance);
        }

        public void Deinitialize()
        {
            this.character = null;
            this.navigation = null;
        }
        
        /// <summary>
        /// Stop Character
        /// </summary>
        public void StopCharacter()
        {
            if (_followToPointsProcess != null) StopCoroutine(_followToPointsProcess);
            
            Character.Stop();
        }

        /// <summary>
        /// Try move to the point by path
        /// </summary>
        /// <param name="point"></param>
        /// <param name="actionAfter"></param>
        /// <returns></returns>
        public bool TryFollowToPoint(Vector2 point, Action actionAfter = null)
        {
            if (Navigation == null) return false;

            if (!Navigation.TryFindPath(Character.position, point, out var path)) return false;
            
            FollowThePath(path, actionAfter);

            return true;
        }
        
        /// <summary>
        /// Try move by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="actionAfter"></param>
        public void FollowThePath(List<Vector2> path, Action actionAfter = null)
        {
            StopCharacter();

            _followToPointsProcess = StartCoroutine(FollowThePathProcess(path, actionAfter));
        }
        
        /// <summary>
        /// Try move by path
        /// </summary>
        /// <param name="path"></param>
        public void FollowThePath(params Vector2[] path)
        {
            FollowThePath(path.ToList());
        }

        #endregion

        #region Private API

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

                    yield return new WaitForFixedUpdate();
                    timer += Time.fixedDeltaTime;
                }
            }

            Character.Stop();
            
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

        #endregion
    }
}