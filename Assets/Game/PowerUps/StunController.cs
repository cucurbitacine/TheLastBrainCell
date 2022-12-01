using System.Collections;
using Game.Characters;
using UnityEngine;
using UnityEngine.Events;

namespace Game.PowerUps
{
    public class StunController : MonoBehaviour
    {
        public UnityEvent<float, float> onStunned;

        [Space]
        public CharacterControllerBase character;
        
        private Coroutine _stunning = null;

        public void Stun(float duration, float speedScale)
        {
            if (_stunning != null) StopCoroutine(_stunning);
            _stunning = StartCoroutine(Stunning(duration, speedScale));
        }

        private IEnumerator Stunning(float duration, float speedScale)
        {
            onStunned.Invoke(duration, speedScale);
            
            character.MoveSetting.speedScale = speedScale;

            yield return new WaitForSeconds(duration);
            
            character.MoveSetting.speedScale = 1f;
        }
    }
}