using System.Linq;
using Game.Characters.Npc;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tools
{
    public class AllNpcDead : MonoBehaviour
    {
        public UnityEvent onAllDead;

        public NpcController[] npcs;

        private void Check()
        {
            if (npcs.All(npc => npc.Health.Value <= 0))
            {
                onAllDead.Invoke();
            }
        }
        
        private void Awake()
        {
            npcs = FindObjectsOfType<NpcController>();

            foreach (var npc in npcs)
            {
                npc.Health.Events.OnValueIsEmpty.AddListener(Check);
            }
        }
    }
}