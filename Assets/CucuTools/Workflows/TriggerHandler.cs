using System.Linq;
using CucuTools.Workflows.Core;

namespace CucuTools.Workflows
{
    public class TriggerHandler : IStateTrigger
    {
        private StateEntity owner;
        
        private IStateTrigger[] conditions;
        private IStateTrigger[] transitions;
        private IStateTrigger[] triggers;
        
        public TriggerHandler(StateEntity owner, params IStateTrigger[] triggers)
        {
            this.owner = owner;

            conditions = triggers.OfType<ConditionEntity>().ToArray();
            transitions = triggers.OfType<TransitionEntity>().Select(t => (IStateTrigger) t).ToArray();
            this.triggers = triggers.OfType<StateTrigger>().ToArray();
        }

        public StateEntity Owner => owner;
        
        public void Invoke(StateInvoke mode)
        {
            foreach (var trn in transitions)
            {
                trn.Invoke(mode);
            }
            
            foreach (var cnd in conditions)
            {
                cnd.Invoke(mode);
            }
            
            foreach (var trg in triggers)
            {
                trg.Invoke(mode);
            }
        }
    }
}