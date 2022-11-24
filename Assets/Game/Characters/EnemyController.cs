using System.Collections;

namespace Game.Characters
{
    public class EnemyController : CharacterControllerBase
    {
        protected override IEnumerator AttackProcess(string attackStateName)
        {
            yield return null;
        }
    }
}