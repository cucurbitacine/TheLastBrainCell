using System.Collections;
using UnityEngine;

namespace Game.Characters
{
    public class PlayerController : CharacterControllerBase
    {
        public string AttackMeleeName => "attack_meleeSimple";
        
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            Animator.Play(attackStateName);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = false;
        }
    }
}