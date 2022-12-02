using CucuTools.Colors;
using Game.AI;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters.Tools
{
    public class DeathController : MonoBehaviour
    {
        public UnityEvent<CharacterControllerBase> onDied;

        public CharacterControllerBase character;

        public void Die()
        {
            DieInternal();
            
            onDied.Invoke(character);
        }

        protected virtual void DieInternal()
        {
            character.Stop();

            foreach (var cld in character.GetComponentsInChildren<Collider2D>())
            {
                cld.enabled = false;
            }

            foreach (var sr in character.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.red.AlphaTo(0.5f);
            }

            var ai = character.GetComponentInChildren<NpcAIController>();

            if (ai != null) ai.enabled = false;
        }
        
        protected virtual void Awake()
        {
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        protected virtual void OnEnable()
        {
            character.Health.Events.OnValueIsEmpty.AddListener(Die);
        }

        protected virtual void OnDisable()
        {
            character.Health.Events.OnValueIsEmpty.RemoveListener(Die);
        }
    }
}
