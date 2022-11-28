using System.Collections;
using UnityEngine;

namespace Game.Characters.Player
{
    public class PlayerController : CharacterControllerBase
    {
        public string AttackMeleeName => "attack_meleeSimple";
        public string AttackMeleeNameDelay => "attack_meleeSimpleDelay";
        
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            Animator.Play(attackStateName);

            var needChange = MoveSetting.ableWhileAttack && string.Equals(attackStateName, AttackMeleeNameDelay);
            
            if (needChange)
            {
                MoveSetting.ableWhileAttack = false;
            }
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = false;
            
            if (needChange)
            {
                MoveSetting.ableWhileAttack = true;
            }
        }
    }
}