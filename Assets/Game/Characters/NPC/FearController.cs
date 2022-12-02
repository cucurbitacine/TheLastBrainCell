using System.Collections;
using UnityEngine;

namespace Game.Characters.Npc
{
    public class FearController : NpcController
    {
        protected override IEnumerator AttackProcess(string attackName)
        {
            Info.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            Info.isAttacking = false;
        }
    }
}