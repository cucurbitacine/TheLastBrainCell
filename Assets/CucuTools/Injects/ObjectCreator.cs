using System;
using UnityEngine;

namespace CucuTools.Injects
{
    public class ObjectCreator : ICanCreateObject
    {
        public static ObjectCreator Instance
        {
            get
            {
                if (_instance == null) _instance = new ObjectCreator();

                return _instance;
            }
        }

        private static ObjectCreator _instance;
        
        public object Create(Type type)
        {
            return type.IsSubclassOf(typeof(MonoBehaviour))
                ? new GameObject(type.Name).AddComponent(type)
                : Activator.CreateInstance(type);
        }

        public T Create<T>()
        {
            return (T) Create(typeof(T));
        }
    }
    
    public interface ICanCreateObject
    {
        object Create(Type type);
        T Create<T>();
    }
}