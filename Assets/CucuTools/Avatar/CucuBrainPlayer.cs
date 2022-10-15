using System;
using UnityEngine;

namespace CucuTools.Avatar
{
    public class CucuBrainPlayer : CucuBrain
    {
        public Vector2 ViewSensitivity = Vector2.one * 1.25f;
        
        public override InputInfo GetInput()
        {
            var input = new InputInfo();

            input.move += Input.GetKey(KeyCode.W) ? Vector3.forward : Vector3.zero;
            input.move += Input.GetKey(KeyCode.A) ? Vector3.left : Vector3.zero;
            input.move += Input.GetKey(KeyCode.S) ? Vector3.back : Vector3.zero;
            input.move += Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero;

            input.view += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            input.view = Vector2.Scale(input.view, ViewSensitivity);
            
            input.sprint = Input.GetKey(KeyCode.LeftShift);
            input.jump = Input.GetKey(KeyCode.Space);
            input.crouch = Input.GetKey(KeyCode.C);
            
            input.sprintDown = input.sprint;
            input.jumpDown = input.jump;
            input.crouchDown = input.crouch;

            return input;
        }
    }
    
    [Serializable]
    public struct InputInfo
    {
        public Vector3 move;
        public Vector2 view;
        
        public bool sprint;
        public bool jump;
        public bool crouch;
        
        public bool sprintDown;
        public bool jumpDown;
        public bool crouchDown;
    }
}