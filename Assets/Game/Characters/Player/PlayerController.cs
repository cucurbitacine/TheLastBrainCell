using System.Collections;
using Game.Audios;
using UnityEngine;

namespace Game.Characters.Player
{
    public class PlayerController : CharacterControllerBase
    {
        [SerializeField] private PlayerAudioController audioSfx = null;

        public PlayerAudioController Audio => audioSfx ??= GetComponentInChildren<PlayerAudioController>();

        public void PlayWeaponSfx() // for animation call
        {
            Audio.weaponSfx.PlayOneShot();
        }
        
        protected override IEnumerator AttackProcess(string attackName)
        {
            Animator.Play(attackName);

            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = true;
            
            var duration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            
            yield return new WaitForEndOfFrame();
            CharacterInfo.isAttacking = false;
        }
    }
}