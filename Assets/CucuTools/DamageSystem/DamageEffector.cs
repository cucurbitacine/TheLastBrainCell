using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CucuTools.DamageSystem
{
    /// <summary>
    /// Behaviour which keep list of <see cref="IDamageEffect"/>
    /// </summary>
    public abstract class DamageEffector : MonoBehaviour, IDamageEffect
    {
        private List<IDamageEffect> _effects { get; } = new List<IDamageEffect>();

        /// <summary>
        /// All kept effects
        /// </summary>
        public IReadOnlyCollection<IDamageEffect> Effects => _effects;

        public void AddEffect(params IDamageEffect[] effects)
        {
            _effects.AddRange(effects.Where(e => !_effects.Contains(e)));
        }
        
        public void RemoveEffect(params IDamageEffect[] effects)
        {
            _effects.RemoveAll(effects.Contains);
        }

        /// <inheritdoc />
        public DamageInfo EvaluateDamage(DamageInfo damage)
        {
            foreach (var effect in _effects)
            {
                damage = effect.EvaluateDamage(damage);
            }
            
            return damage;
        }
    }

    /// <summary>
    /// Some effect which able to evaluate damage
    /// </summary>
    public interface IDamageEffect
    {
        /// <summary>
        /// Evaluating damage
        /// </summary>
        /// <param name="damage">Input damage</param>
        /// <returns>Output damage</returns>
        DamageInfo EvaluateDamage(DamageInfo damage);
    }
}