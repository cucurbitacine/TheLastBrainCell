using System.Linq;
using Game.Characters.Npc;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tools
{
    public class WinCondition : MonoBehaviour
    {
        public UnityEvent onWin;

        public bool win;
        public NpcController[] npcs;

        private void Awake()
        {
            npcs = FindObjectsOfType<NpcController>();
        }

        private void Update()
        {
            if (!win)
            {
                win = npcs.All(npc => npc.Health.Value <= 0);
                if (win) onWin.Invoke();
            }
        }
    }
}