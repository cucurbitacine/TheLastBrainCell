using CucuTools.Attributes;
using UnityEngine;

namespace Game.Inputs.Combos
{
    [CreateAssetMenu(menuName = "Create ComboController", fileName = "ComboController", order = 0)]
    public class ComboEntity : ScriptableObject
    {
        [CucuReadOnly]
        public int max;
        
        [Space]
        public AttackEntity[] attacks;

        private void Validate()
        {
            max = attacks?.Length ?? 0;

            if (attacks != null)
            {
                foreach (var combo in attacks)
                {
                    combo.Validate();
                }
            }
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