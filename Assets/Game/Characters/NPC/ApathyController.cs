using System.Collections;
using UnityEngine;

namespace Game.Characters.Npc
{
    public class ApathyController : NpcController
    {
        [Space]
        public ApathyMissileSettings missileSettings = new ApathyMissileSettings();
        public Vector2 missileSpawnLocalPoint = Vector2.up; 
        public ApathyMissile missilePrefab = null;

        public void Fire()
        {
            var pos = transform.TransformPoint(missileSpawnLocalPoint);
            var rot = rotation;
            
            var missile = Instantiate<ApathyMissile>(missilePrefab, pos, rot);
                
            missile.Fire(missileSettings);
        }
        
        protected override IEnumerator AttackProcess(string attackName)
        {
            CharacterInfo.isAttacking = true;
            
            Fire();
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            CharacterInfo.isAttacking = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(missileSpawnLocalPoint), 0.1f);
        }
    }
}