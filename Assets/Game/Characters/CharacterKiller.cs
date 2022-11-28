using UnityEngine;
using UnityEngine.Events;

namespace Game.Characters
{
    public class CharacterKiller : MonoBehaviour
    {
        public UnityEvent<CharacterControllerBase> onKilled;

        public CharacterControllerBase character;

        public void Kill()
        {
            KillInternal();
            
            onKilled.Invoke(character);
        }

        protected virtual void KillInternal()
        {
            character.Stop();

            foreach (var cld in character.GetComponents<Collider2D>())
            {
                cld.enabled = false;
            }

            character.GetComponent<SpriteRenderer>().color = Color.red;
        }
        
        protected virtual void Awake()
        {
            if (character == null) character = GetComponentInParent<CharacterControllerBase>();
        }

        protected virtual void OnEnable()
        {
            character.Health.Events.OnValueIsEmpty.AddListener(Kill);
        }

        protected virtual void OnDisable()
        {
            character.Health.Events.OnValueIsEmpty.RemoveListener(Kill);
        }
    }
}