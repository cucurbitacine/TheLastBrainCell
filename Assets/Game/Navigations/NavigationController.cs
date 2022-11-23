using System.Collections.Generic;
using Aoiti.Pathfinding;
using UnityEngine;

namespace Game.Navigations
{
    [DisallowMultipleComponent]
    public class NavigationController : MonoBehaviour
    {
        private static NavigationController _instance = null;

        public static NavigationController Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var navigations = FindObjectsOfType<NavigationController>();

                for (var i = 0; i < navigations.Length; i++)
                {
                    if (navigations[i].isSingleton)
                    {
                        _instance = navigations[i];
                        return _instance;
                    }
                }
                
                _instance = new GameObject(nameof(NavigationController)).AddComponent<NavigationController>();
                
                DontDestroyOnLoad(_instance.gameObject);
                _instance.isSingleton = true;
                
                return _instance;
            }
        }
        
        [Header("Navigator options")]
        [Min(0.1f)]
        [SerializeField] private float gridSize = 1f; //increase patience or gridSize for larger maps

        [Tooltip("The layers that the navigator can not pass through.")]
        [SerializeField] private LayerMask obstacleLayer = 1;

        [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")]
        [SerializeField] private bool searchShortcut = true;

        [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
        [SerializeField] private bool snapToGrid = false;

        [SerializeField] private int patience = 1000;

        [Space]
        
        [SerializeField] private bool isSingleton = false;
        
        private Pathfinder<Vector2> _pathfinder; //the pathfinder object that stores the methods and patience
        
        private Pathfinder<Vector2> pathfinder => _pathfinder ??= new Pathfinder<Vector2>(GetDistance, GetNeighbourNodes, Patience);
        
        public float GridSize
        {
            get => gridSize;
            set => gridSize = value;
        }

        public LayerMask ObstacleLayer
        {
            get => obstacleLayer;
            set => obstacleLayer = value;
        }

        public bool SearchShortcut
        {
            get => searchShortcut;
            set => searchShortcut = value;
        }

        public bool SnapToGrid
        {
            get => snapToGrid;
            set => snapToGrid = value;
        }

        public int Patience
        {
            get => patience;
            set => patience = value;
        }

        public bool TryFindPath(Vector2 start, Vector2 target, out List<Vector2> path)
        {
            if (!pathfinder.GenerateAstarPath(GetClosestNode(start), GetClosestNode(target), out path)) return false;
            //Generate path between two points on grid that are close to the transform position and the assigned target.

            if (!SnapToGrid) path.Add(target);

            if (SearchShortcut) path = ShortenPath(path);

            return true;
        }

        /// <summary>
        /// Finds closest point on the grid
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector2 GetClosestNode(Vector2 target)
        {
            return new Vector2(Mathf.Round(target.x / GridSize) * GridSize, Mathf.Round(target.y / GridSize) * GridSize);
        }

        /// <summary>
        /// A distance approximation. 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private float GetDistance(Vector2 A, Vector2 B)
        {
            return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
        }

        /// <summary>
        /// Finds possible connections and the distances to those connections on the grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Dictionary<Vector2, float> GetNeighbourNodes(Vector2 pos)
        {
            var neighbours = new Dictionary<Vector2, float>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue;

                    var dir = new Vector2(i, j) * GridSize;
                    if (!Physics2D.CircleCast(pos, GridSize * 0.5f, dir.normalized, dir.magnitude, ObstacleLayer))
                    {
                        neighbours.Add(GetClosestNode(pos + dir), dir.magnitude);
                    }
                }
            }

            return neighbours;
        }

        private List<Vector2> ShortenPath(IReadOnlyList<Vector2> points)
        {
            var newPath = new List<Vector2>();

            for (var i = 0; i < points.Count; i++)
            {
                var pos = points[i];

                newPath.Add(points[i]);
                for (var j = points.Count - 1; j > i; j--)
                {
                    var dir = points[j] - points[i];
                    if (!Physics2D.CircleCast(pos, GridSize * 0.5f, dir.normalized, dir.magnitude, ObstacleLayer))
                    {
                        i = j;
                        break;
                    }
                }

                newPath.Add(points[i]);
            }

            //newPath.Add(points[points.Count - 1]);

            return newPath;
        }

        private void Awake()
        {
            if (isSingleton)
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}