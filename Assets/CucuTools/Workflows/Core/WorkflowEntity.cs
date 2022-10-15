namespace CucuTools.Workflows.Core
{
    public abstract class WorkflowEntity : StateEntity
    {
        public abstract bool Paused { get; set; }
        
        public abstract StateEntity Current { get; }

        public StateEntity GetDeepCurrentState()
        {
            return GetDeepCurrentState(this);
        }
        
        public static StateEntity GetDeepCurrentState(StateEntity root)
        {
            return root is WorkflowEntity workflow ? GetDeepCurrentState(workflow.Current) : root;
        }
    }
}