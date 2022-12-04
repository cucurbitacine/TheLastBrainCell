using CucuTools.Attributes;
using UnityEngine;

namespace Game.Inputs.Combos
{
    [CreateAssetMenu(menuName = menuName, fileName = fileName, order = 0)]
    public class ComboEntity : ScriptableObject
    {
        public const string title = "Combo system";
        public const string menuName = GameManager.title + "/" + title + "/Create " + fileName;
        public const string fileName = nameof(ComboEntity);
        
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