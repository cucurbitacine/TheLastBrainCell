using System.Collections;
using UnityEngine;

namespace Game.Characters.Npc
{
    public class FearController : NpcController
    {
        protected override IEnumerator AttackProcess(string attackName)
        {
            CharacterInfo.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            CharacterInfo.isAttacking = false;
        }
    }
}