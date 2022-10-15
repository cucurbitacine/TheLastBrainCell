using System.Collections;
using UnityEngine;

namespace Game.Characters
{
    public class PlayerController : CharacterController
    {
        public string AttackMeleeName => "attack_meleeSimple";
        
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            Animator.Play(attackStateName);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = true;
            
            AttackSetting.duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(AttackSetting.duration);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = false;
        }
    }
}