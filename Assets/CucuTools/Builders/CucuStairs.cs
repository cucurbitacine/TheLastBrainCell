using System.Linq;
using CucuTools.Attributes;
using UnityEngine;

namespace CucuTools.Builders
{
    public class CucuStairs : CucuBehaviour
    {
        [CucuReadOnly]
        [SerializeField] private Vector2 stepSize;
        
        [Header("Settings")]
        public bool FakeStairs = false;
        [Min(0)]
        public float Depth = 1f;
        [Min(0)]
        public float Height = 1f;
        [Min(0)]
        public float Width = 1f;
        
        [Min(1)]
        public int StepsCount = 5;

        [Header("Root & Models")]
        [SerializeField] private GameObject stepPrefab;
        [SerializeField] private Transform root;
        
        public GameObject StepPrefab
        {
            get => stepPrefab != null ? stepPrefab : (stepPrefab = Resources.Load<GameObject>("CucuStepPrefab"));
            set => stepPrefab = value;
        }

        public Transform Root
        {
            get => root != null ? root : (root = transform);
            set => root = value;
        }

        public float StepDepth => Depth / StepsCount;
        public float StepHeight => Height / StepsCount;

        [CucuButton()]
        [ContextMenu(nameof(Build))]
        public void Build()
        {
            var children = Root.GetComponentsInChildren<Transform>()
                .Where(t => t.parent == Root)
                .ToArray();
            
            foreach (var child in children)
            {
                Cucu.Destroy(child.gameObject);
            }

            if (StepPrefab == null) return;

            for (int i = 0; i < StepsCount; i++)
            {
                var step = Instantiate(StepPrefab, Root, false).transform;

                step.localScale = new Vector3(Width, StepHeight, StepDepth);

                var localPosition = Vector3.zero;
                localPosition += Vector3.forward * (StepDepth * 0.5f + StepDepth * i);
                localPosition += Vector3.up * (Height - StepHeight * 0.5f - StepHeight * i);
                step.localPosition = localPosition;

                var stepCollider = step.GetComponent<Collider>();
                
                if (FakeStairs)
                {
                    if (stepCollider != null) stepCollider.enabled = false;
                }
                else
                {
                    if (stepCollider == null) stepCollider = step.gameObject.AddComponent<BoxCollider>();
                }
            }

            if (FakeStairs)
            {
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                quad.SetParent(Root, false);

                var length2 = Mathf.Pow(Height, 2) + Mathf.Pow(Depth, 2);
                var offset = Mathf.Sqrt(Mathf.Pow(Height, 4) / (length2));
                var dir = (Vector3.forward * Depth - Vector3.up * Height).normalized;

                var shift = dir * offset;
                var point = Vector3.up * Height + shift;
                var frw = (Vector3.zero - point).normalized;
                var up = -dir;
                    
                var localRotation = Quaternion.LookRotation(frw, up);
                quad.localRotation = localRotation;
                quad.localPosition = (Vector3.forward * Depth + Vector3.up * Height) * 0.5f;
                quad.localScale = new Vector3(Width, Mathf.Sqrt(length2), 1f);

                quad.gameObject.GetComponent<Renderer>().enabled = false;

                quad = Instantiate(quad, quad, false);
                quad.localPosition = Vector3.zero;
                quad.localRotation = Quaternion.Euler(0, 180f, 0);
                quad.localScale = Vector3.one;
            }
        }

        private void OnValidate()
        {
            stepSize.x = Depth / StepsCount;
            stepSize.y = Height / StepsCount;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Root.position - Root.right * Width * 0.5f, Root.position + Root.right * Width * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Root.position, Root.position + Root.up * Height);
            Gizmos.DrawLine(Root.position - Root.right * Width * 0.5f, Root.position - Root.right * Width * 0.5f + Root.up * Height);
            Gizmos.DrawLine(Root.position + Root.right * Width * 0.5f, Root.position + Root.right * Width * 0.5f + Root.up * Height);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Root.position, Root.position + Root.forward * Depth);
            Gizmos.DrawLine(Root.position - Root.right * Width * 0.5f, Root.position - Root.right * Width * 0.5f + Root.forward * Depth);
            Gizmos.DrawLine(Root.position + Root.right * Width * 0.5f, Root.position + Root.right * Width * 0.5f + Root.forward * Depth);
            
            Gizmos.color = Color.yellow;
            for (int j = 0; j < 3; j++)
            {
                var start = Vector3.zero + Vector3.right * (j - 1) * Width * 0.5f;
                
                for (int i = 0; i < StepsCount; i++)
                {
                    var a = start;
                    a += Vector3.forward * (StepDepth * i);
                    a += Vector3.up * (Height - StepHeight * i);

                    var b = a + Vector3.forward * StepDepth;
                    var c = b + Vector3.down * StepHeight;

                    a = Root.TransformPoint(a);
                    b = Root.TransformPoint(b);
                    c = Root.TransformPoint(c);
                
                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(b, c);
                }
            }
            
        }
    }
}