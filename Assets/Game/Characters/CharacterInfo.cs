using System;
using UnityEngine;
using UnityEngine.Events;

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
    public abstract class BaseSetting
    {
        public bool enabled = true;
    }
    
    [Serializable]
    public class MoveSetting : BaseSetting
    {
        [Space]
        [Min(0.001f)] public float speedMax = 6f;
        public bool ableWhileAttack = false;
        [Min(0.001f)] public float damp = 32f;
        
        [Space]
        public Vector2 direction = Vector2.zero;

        public Vector2 velocity => direction * speedMax;
    }
    
    [Serializable]
    public class ViewSetting : BaseSetting
    {
        [Space]
        public bool ableWhileAttack = false;
        [Min(0.001f)] public float damp = 32f;

        [Space]
        public Vector2 direction = Vector2.zero;
    }
    
    [Serializable]
    public class JumpSetting : BaseSetting
    {
        [Space] 
        [Min(0.001f)] public float speed = 12f;
        [Min(0.001f)] public float distance = 4f;
        
        [Space]
        public bool useStamina = true;
        public int staminaCost = 1;
        
        [Space]
        public UnityEvent onJumped = new UnityEvent();
        
        public float duration => distance / speed;
    }
    
    [Serializable]
    public class AttackSetting : BaseSetting
    {
        [Space]
        public bool ableWhileAttack = false;
        
        [Space]
        public bool useStamina = true;
        public int staminaCost = 1;

        [Space]
        public UnityEvent onAttacked = new UnityEvent();
    }
}