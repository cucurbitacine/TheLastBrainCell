using System.Collections;
using UnityEngine;

namespace Game.Characters.Player
{
    public class PlayerController : CharacterControllerBase
    {
        [SerializeField] private PlayerAudioController audioSfx = null;

        public PlayerAudioController Audio => audioSfx ??= GetComponentInChildren<PlayerAudioController>();

        public string JumpAttackName => "player_jumpAttack";
        
        public void PlayWeaponSfx() // for animation call
        {
            Audio.weaponSfx.Play();
        }
        
        protected override IEnumerator AttackProcess(string attackName)
        {
            Animator.Play(attackName);

            yield return new WaitForEndOfFrame();
            Info.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            yield return new WaitForEndOfFrame();
            Info.isAttacking = false;
        }
    }
}