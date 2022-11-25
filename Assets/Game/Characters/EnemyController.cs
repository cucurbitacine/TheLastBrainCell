using System.Collections;
using UnityEngine;

namespace Game.Characters
{
    public class EnemyController : CharacterControllerBase
    {
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            Debug.Log($"[{name}] {attackStateName}");
            
            yield return null;
        }
    }
}