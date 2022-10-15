namespace CucuTools.Workflows.Core
{
    public abstract class TransitionEntity : OwnOfState
    {
        public abstract ConditionEntity[] Conditions { get; set; }
        
        public abstract bool IsReady { get; }

        public abstract StateEntity Target { get; set; }
    }
}