using UnityEngine;

namespace Game.Inputs.Combos
{
    [CreateAssetMenu(menuName = "Create ComboUnit", fileName = "ComboUnit", order = 0)]
    public class AttackEntity : ScriptableObject
    {
        public string attackName = string.Empty;
        
        [Space]
        [Min(-1f)]
        public float duration = -1f;
        [Min(0f)]
        public float sleep = 0f;
        
        [Space]
        public AnimationClip animationClip = null;
        
        public void Validate()
        {
            if (animationClip != null)
            {
                if (duration < 0) duration = animationClip.length;
                if (attackName == "") attackName = animationClip.name;
            }

            sleep = Mathf.Min(sleep, duration);
        }
        
        private void Awake()
        {
            Validate();
        }

        private void OnValidate()
        {
            Validate();
        }
    }
}