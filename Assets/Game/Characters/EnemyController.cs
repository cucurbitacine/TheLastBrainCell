using System.Collections;

namespace Game.Characters
{
    public class EnemyController : CharacterController
    {
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            yield return null;
        }
    }
}