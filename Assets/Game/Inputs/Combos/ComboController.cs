using CucuTools.Attributes;
using UnityEngine;

namespace Game.Inputs.Combos
{
    public class ComboController : MonoBehaviour
    {
        [CucuReadOnly]
        public int numberAttack = -1;
        
        [Space]
        [CucuReadOnly]
        public float timer = 0f;
        
        [Space]
        public ComboEntity combo = null;
        
        public bool Attack(out string attackName)
        {
            // first combo
            if (numberAttack < 0)
            {
                NextCombo(out attackName);

                return true;
            }

            // last combo
            if (numberAttack >= combo.max - 1)
            {
                attackName = null;
                return false;
            }
            
            if (IsTimerValid())
            {
                NextCombo(out attackName);

                return true;
            }
            
            attackName = null;
            return false;
        }
        
        public bool IsTimerValid()
        {
            var currentCombo = combo.attacks[numberAttack];
            
            var maxTimer = currentCombo.duration - currentCombo.sleep;
            
            return timer <= maxTimer;
        }
        
        private void NextCombo(out string attackName)
        {
            numberAttack++;
                
            var nextCombo = combo.attacks[numberAttack];
            attackName = nextCombo.attackName;
            timer = nextCombo.duration;
        }

        private void ResetCombo()
        {
            numberAttack = -1;
            timer = 0f;
        }
        
        private void UpdateController(float deltaTime)
        {
            if (timer > 0)
            {
                timer -= deltaTime;
            }

            if (timer < 0)
            {
                ResetCombo();
            }
        }

        private void Awake()
        {
            numberAttack = -1;
        }

        private void Update()
        {
            UpdateController(Time.deltaTime);
        }
    }
}