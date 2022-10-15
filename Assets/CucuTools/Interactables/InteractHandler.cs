using System;
using System.Collections.Generic;
using System.Linq;

namespace CucuTools.Interactables
{
    public class InteractHandler
    {
        public ICucuContext Context { get; private set; }
        public bool Pressing { get; private set; }

        public IReadOnlyList<IInteractableEntity> Currents => currents;
        public IReadOnlyList<IInteractableEntity> Previous => previous;

        private readonly List<IInteractableEntity> currents = new List<IInteractableEntity>();
        private readonly List<IInteractableEntity> previous = new List<IInteractableEntity>();

        private readonly Dictionary<IInteractableEntity, bool> canPressing = new Dictionary<IInteractableEntity, bool>();
        private readonly Dictionary<IInteractableEntity, bool> wasPressing = new Dictionary<IInteractableEntity, bool>();

        private readonly IInteractableEntity[] emptyArray = Array.Empty<IInteractableEntity>();
        
        public void Update(ICucuContext context, bool pressing, params IInteractableEntity[] interactables)
        {
            Update(context, pressing, interactables?.Where(i => i != null));
        }
        
        public void Update(ICucuContext context, bool pressing, IEnumerable<IInteractableEntity> interactables)
        {
            Context = context;
            Pressing = pressing;

            previous.Clear();
            previous.AddRange(currents);

            currents.Clear();
            currents.AddRange(interactables ?? emptyArray);

            previous.ForEach(UpdatePrevious);
            currents.ForEach(UpdateCurrents);
        }

        private void UpdateCurrents(IInteractableEntity current)
        {
            if (!wasPressing.ContainsKey(current)) wasPressing[current] = false;
            canPressing[current] = current.InteractInfo.Press || (current.InteractInfo.Hover && !wasPressing[current]);

            if (canPressing[current] && Pressing) current.Press(Context);
            else current.Hover(Context);

            wasPressing[current] = Pressing;
        }

        private void UpdatePrevious(IInteractableEntity prev)
        {
            if (!currents.Contains(prev)) prev.Idle();
        }
    }
}