using System;
using UnityEngine;

namespace Game.Characters
{
    [Serializable]
    public class CharacterInfo
    {
        public bool isMoving = false;
        public bool isJumping = false;
        public bool isAttacking = false;
        
        public Vector2 velocityMove = Vector2.zero;
        public Vector2 velocityJump = Vector2.zero;
        
        public Quaternion rotationView = Quaternion.identity;
        
        public Vector2 velocityTotal => velocityMove + velocityJump;
    }
    
    [Serializable]
    public class MoveSetting
    {
        public bool enabled = true;
        [Space]
        [Min(0.001f)] public float speedMax = 6f;
        [Min(0.001f)] public float damp = 32f;
        
        public Vector2 direction = Vector2.zero;

        public Vector2 velocity => direction * speedMax;
    }
    
    [Serializable]
    public class ViewSetting
    {
        public bool enabled = true;
        [Space]
        [Min(0.001f)] public float damp = 32f;
        
        public Vector2 direction = Vector2.zero;
    }
    
    [Serializable]
    public class JumpSetting
    {
        public bool enabled = true;
        [Space] 
        [Min(0.001f)] public float speed = 12f;
        [Min(0.001f)] public float distance = 4f;

        public float duration => distance / speed;
    }
    
    [Serializable]
    public class AttackSetting
    {
        public bool enabled = true;
        [Space]
        public float duration = 0f;
    }
}