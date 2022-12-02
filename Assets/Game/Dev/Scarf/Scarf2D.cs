using CucuTools;
using UnityEngine;

namespace Game.Dev.Scarf
{
    public class Scarf2D : MonoBehaviour
    {
        public float lengthScarf = 2f;
        public int depthSimulation = 8;

        [Space]
        public LineRenderer line;

        private Point2D _lockedPoint;
        private const int CountConnections = 32;
        private Point2D[] _points;
        private Connection2D[] _connections;

        private void CreateScarf()
        {
            var pos = (Vector2)transform.position;
            
            var scarf = Cucu.LinSpace(0, lengthScarf, CountConnections + 1);

            _connections = new Connection2D[CountConnections];
            _points = new Point2D[scarf.Length];
            line.positionCount = _points.Length;
            
            for (var i = 0; i < _points.Length; i++)
            {
                _points[i] = new Point2D(pos + Vector2.down * scarf[i]);
                
                if (i > 0)
                {
                    _connections[i - 1] = new Connection2D(_points[i - 1], _points[i]);
                }
                
                line.SetPosition(i, _points[i].position);
            }

            _lockedPoint = _points[0];
            _lockedPoint.locked = true;
        }

        private void UpdateLine()
        {
            for (var i = 0; i < _points.Length; i++)
            {
                line.SetPosition(i, _points[i].position);
            }
        }

        private void SimulateConnections()
        {
            /*
             * Simulate physics per connection
             */

            _lockedPoint.previousPosition = _lockedPoint.position;
            _lockedPoint.position = transform.position;
            
            for (var i = 0; i < depthSimulation; i++)
            {
                foreach (var connection in _connections)
                {
                    connection.Simulate();
                }
            }
        }
        
        private void Awake()
        {
            CreateScarf();
        }

        private void Update()
        {
            UpdateLine();
        }

        private void FixedUpdate()
        {
            SimulateConnections();
        }
    }
}