using System;
using System.Collections.Generic;
using System.Linq;

namespace CucuTools.Injects
{
    /// <summary>
    /// <see cref="CucuArg"/> list manager
    /// </summary>
    [Serializable]
    public class CucuArgumentManager
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static CucuArgumentManager Singleton { get; }

        /// <summary>
        /// Name of manager
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Arguments list
        /// </summary>
        private List<CucuArg> Args => _args ?? (_args = new List<CucuArg>());

        private List<CucuArg> _args;
        
        static CucuArgumentManager()
        {
            Singleton = new CucuArgumentManager("Singleton");
        }

        public CucuArgumentManager(string name)
        {
            Name = name;
        }
        
        /// <summary>
        /// Set list of arguments. Clear old list before set. 
        /// </summary>
        /// <param name="args">List of possible arguments</param>
        public void SetArguments(params CucuArg[] args)
        {
            Clear();
            
            AddArguments(args);
        }

        /// <summary>
        /// Add arguments to list
        /// </summary>
        /// <param name="args">List of possible arguments</param>
        public void AddArguments(params CucuArg[] args)
        {
            if (args == null) return;
            Args.AddRange(args.Where(a => a != null));
        }

        /// <summary>
        /// Clear list
        /// </summary>
        public void Clear()
        {
            Args.Clear();
        }
        
        /// <summary>
        /// Getting arguments of type <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">Argument type</typeparam>
        /// <returns>List of arguments</returns>
        public T[] GetArgs<T>()
        {
            return Args.OfType<T>().ToArray();
        }

        /// <summary>
        /// Getting arguments of type <param name="argType"></param>.
        /// If type is null - return all arguments
        /// </summary>
        /// <param name="argType">Argument type</param>
        /// <returns>List of arguments</returns>
        public CucuArg[] GetArgs(Type argType = null)
        {
            if (argType == null) return Args.ToArray();

            return Args.Where(a => a.GetType() == argType).ToArray();
        }

        /// <summary>
        /// Trying get arguments of type <see cref="T"/>
        /// </summary>
        /// <param name="args">Result list of arguments</param>
        /// <typeparam name="T">Argument type</typeparam>
        /// <returns>Success</returns>
        public bool TryGetArgs<T>(out T[] args) where T : CucuArg
        {
            return (args = GetArgs<T>()) != null;
        }

        /// <summary>
        /// Trying get arguments of type <param name="argType"></param>
        /// </summary>
        /// <param name="argType">Result list of arguments</param>
        /// <param name="args">Argument type</param>
        /// <returns>Success</returns>
        public bool TryGetArgs(Type argType, out CucuArg[] args)
        {
            return (args = GetArgs(argType)) != null;
        }
    }
}