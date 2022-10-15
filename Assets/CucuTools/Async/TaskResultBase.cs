using System.Threading.Tasks;

namespace CucuTools.Async
{
    public abstract class TaskResultBase<T>
        where T : Task
    {
        public T Task { get;  }
        public bool TimeOut { get; }

        public TaskResultBase(T task, bool timeOut)
        {
            Task = task;
            TimeOut = timeOut;
        }
    }
    
    public class TaskResult : TaskResultBase<Task>
    {
        public static explicit operator Task(TaskResult tr) => tr.Task;

        public TaskResult(Task task, bool timeOut) : base(task, timeOut)
        {
        }
    }
    
    public class TaskResult<T> : TaskResultBase<Task<T>>
    {
        public static explicit operator Task<T>(TaskResult<T> tr) => tr.Task;

        public T Result => Task.Result;
        
        public TaskResult(Task<T> task, bool timeOut) : base(task, timeOut)
        {
        }
    }
}