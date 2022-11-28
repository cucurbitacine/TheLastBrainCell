using Game.Characters.Npc;
using Game.Inputs;

namespace Game.AI
{
    /// <summary>
    /// Base npc AI controller
    /// </summary>
    public abstract class NpcAIController : InputController
    {
        public bool debugMode = false;

        public NpcController npc;
    }
}