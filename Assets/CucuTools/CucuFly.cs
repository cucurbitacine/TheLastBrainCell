using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CucuTools
{
    public class CucuFly : CucuBehaviour
    {
        private Vector3 velocity;
        
        private readonly Dictionary<KeyCode, Vector3> control = new Dictionary<KeyCode, Vector3>()
        {
            { KeyCode.W, Vector3.forward },
            { KeyCode.A, Vector3.left },
            { KeyCode.S, Vector3.back },
            { KeyCode.D, Vector3.right },
            { KeyCode.E, Vector3.up },
            { KeyCode.Q, Vector3.down },
        };

        private const float AccelerationMin = 0f;
        private const float AccelerationMax = 128f;
        
        private const float SprintScaleMin = 0f;
        private const float SprintScaleMax = 8f;
        
        private const float DampingMin = 0f;
        private const float DampingMax = 16f;
        
        private const float ViewScaleMin = 0f;
        private const float ViewScaleMax = 16f;
        
        #region SerializeField

        [SerializeField] private bool isEnabled = true;
        [Range(AccelerationMin, AccelerationMax)]
        [SerializeField] private float acceleration = 32f;
        [Range(SprintScaleMin, SprintScaleMax)]
        [SerializeField] private float sprintScale = 4f;
        [Range(DampingMin, DampingMax)]
        [SerializeField] private float damping = 8f;
        [Range(ViewScaleMin, ViewScaleMax)]
        [SerializeField] private float viewScale = 1f;
        [SerializeField] private bool useFixedUpdate;

        #endregion

        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public float Acceleration
        {
            get => acceleration;
            set => acceleration = Mathf.Clamp(value, AccelerationMin, AccelerationMax);
        }

        public float SprintScale
        {
            get => sprintScale;
            set => sprintScale = Mathf.Clamp(value, SprintScaleMin, SprintScaleMax);
        }

        public float Damping
        {
            get => damping;
            set => damping = Mathf.Clamp(value, DampingMin, DampingMax);
        }
        
        public float ViewScale
        {
            get => viewScale;
            set => viewScale = Mathf.Clamp(value, ViewScaleMin, ViewScaleMax);
        }

        public bool UseFixedUpdate
        {
            get => useFixedUpdate;
            set => useFixedUpdate = value;
        }

        public static bool Focused
        {
            get => Cursor.lockState == CursorLockMode.Locked;
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = Cursor.lockState == CursorLockMode.None;
            }
        }

        private Vector3 EvaluateAcceleration()
        {
            Vector3 moveInput = control.Where(ctrl => Input.GetKey(ctrl.Key)).Aggregate(Vector3.zero, (current, ctrl) => current + ctrl.Value);

            Vector3 direction = transform.TransformVector(moveInput.normalized);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                return direction * (Acceleration * SprintScale); 
            }
            
            return direction * Acceleration; 
        }
        
        private void UpdateInput(float deltaTime)
        {
            velocity += EvaluateAcceleration() * deltaTime;

            var view = ViewScale * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            var rot = transform.rotation;
            var rotX = Quaternion.AngleAxis(view.x, Vector3.up);
            var rotY = Quaternion.AngleAxis(view.y, Vector3.right);
            transform.rotation = rotX * rot * rotY;

            if (Input.GetKeyDown(KeyCode.Escape)) Focused = false;
        }
        
        private void UpdateFly(float deltaTime)
        {
            Focused = Input.GetKey(KeyCode.Mouse1);
            
            if (Focused) UpdateInput(deltaTime);
            
            velocity = Vector3.Lerp(velocity, Vector3.zero, Damping * deltaTime);
            transform.position += velocity * deltaTime;
        }

        private void Update()
        {
            if (!UseFixedUpdate)
            {
                if (IsEnabled) UpdateFly(Time.deltaTime);
                else if (Focused) Focused = false;
            }
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate)
            {
                if (IsEnabled) UpdateFly(Time.fixedDeltaTime);
                else if (Focused) Focused = false;
            }
        }
    }
}