using Game.Audios;
using UnityEngine;

namespace Game.Characters.Player
{
    public class PlayerAudioController : MonoBehaviour
    {
        public AudioSFX weaponSfx;
        
        public void PlayWeaponSfx()
        {
            weaponSfx.PlayOneShot();   
        }
    }
}
