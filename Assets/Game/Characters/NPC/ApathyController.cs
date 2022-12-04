using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Characters.Npc
{
    public class ApathyController : NpcController
    {
        [Space]
        public ApathyMissileSettings missileSettings = new ApathyMissileSettings();
        public Vector2 missileSpawnLocalPoint = Vector2.up; 
        public ApathyMissile missilePrefab = null;

        private readonly List<ApathyMissile> _missiles = new List<ApathyMissile>();
        
        public void Fire()
        {
            var pos = transform.TransformPoint(missileSpawnLocalPoint);
            var rot = rotation;

            var missile = _missiles.FirstOrDefault(m => !m.active);
            if (missile == null)
            {
                missile = Instantiate<ApathyMissile>(missilePrefab);
                _missiles.Add(missile);
            }

            missile.transform.position = pos;
            missile.transform.rotation = rot;
            
            missile.Fire(missileSettings);
        }
        
        protected override IEnumerator AttackProcess(string attackName)
        {
            Info.isAttacking = true;
            
            Fire();
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            Info.isAttacking = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(missileSpawnLocalPoint), 0.1f);
        }
    }
}