using System.Collections;
using UnityEngine;

namespace Game.Characters
{
    public class EnemyController : CharacterControllerBase
    {
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            CharacterInfo.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            CharacterInfo.isAttacking = false;
        }
    }
}