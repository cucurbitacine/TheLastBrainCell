using UnityEngine;

namespace CucuTools.Avatar
{
    public class CucuBrainPlayer2D : CucuBrain2D
    {
        public Camera Camera;
    
        protected override InputInfo2D GetInput()
        {
            var input = new InputInfo2D();

            input.move.x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            input.move.y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);

            input.view = transform.InverseTransformPoint(Camera.ScreenToWorldPoint(Input.mousePosition));

            input.climbDown = input.move.y < -0.5f && Input.GetKey(KeyCode.Space);

            input.jump = !input.climbDown && Input.GetKey(KeyCode.Space);

            input.sprint = Input.GetKey(KeyCode.LeftShift);
            
            return input;
        }
    }
}