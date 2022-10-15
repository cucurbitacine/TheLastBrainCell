using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CucuTools.Injects
{
    [Serializable]
    public class CucuContainer : IContainer
    {
        private Dictionary<Type, IBinder> Map => map ?? (map = new Dictionary<Type, IBinder>());
        private Dictionary<Type, IBinder> map;
        
        public CucuContainer(string key = null)
        {
            Guid = Guid.NewGuid();
            Key = key ?? Guid.ToString();

            map = new Dictionary<Type, IBinder>();
        }

        public Type[] GetBindTypes()
        {
            return Map.Select(m => m.Key).ToArray();
        }

        public IBinder Bind(Type type)
        {
            Map[type] = new CucuBinder(type);
            return Map[type];
        }

        public IBinder Bind<T>()
        {
            return Bind(typeof(T));
        }

        public void Unbind(Type type)
        {
            Map.Remove(type);
        }

        public void Unbind<T>()
        {
            Unbind(typeof(T));
        }

        public bool TryResolve(Type type, out object result)
        {
            if (Map.TryGetValue(type, out var bind))
            {
                result = bind.Factory.Get();
                return true;
            }

            result = default;
            return false;
        }

        public bool TryResolve<T>(out T result)
        {
            if (Map.TryGetValue(typeof(T), out var bind))
            {
                result = bind.Factory.Get<T>();
                return true;
            }

            result = default;
            return false;
        }

        public object Resolve(Type type)
        {
            return TryResolve(type, out var result) ? result : default;
        }

        public T Resolve<T>()
        {
            return TryResolve<T>(out var result) ? result : default;
        }

        public Guid Guid { get; }
        public string Key
        {
            get => key;
            private set => key = value;
        }

        [SerializeField] private string key;
    }

    public class CucuBinder : IBinder
    {
        public IObjectFactory Factory { get; protected set; }

        public Type BindType { get; }

        public CucuBinder(Type type)
        {
            Validation(type);
            
            BindType = type;
        }

        public void To(Type type)
        {
            ValidationTo(type);

            Factory = new CucuFactoryCreate(type);
        }

        public void To<T>()
        {
            To(typeof(T));
        }

        public void ToSingleton(Type type)
        {
            ValidationToSingleton(type);

            var obj = ObjectCreator.Instance.Create(type);
            if (obj is MonoBehaviour behaviour && Application.isPlaying) Object.DontDestroyOnLoad(behaviour.gameObject);

            ToInstance(type, obj);
        }

        public void ToSingleton<T>()
        {
            ToSingleton(typeof(T));
        }

        public void ToInstance(Type type, object instance)
        {
            ValidationToInstance(type, instance);
            
            Factory = new CucuFactoryInstance(type, instance);
        }

        public void ToInstance<T>(T instance)
        {
            ToInstance(typeof(T), instance);
        }

        public void ToSelf()
        {
            To(BindType);
        }

        private void Validation(Type type)
        {
            if (type == null) throw new Exception(GetTypeIsNullExcMsg());
        }
        
        private void ValidationTo(Type type)
        {
            if (type == null) throw new Exception(GetTypeIsNullExcMsg());
            if (type.IsInterface || type.IsAbstract) throw new Exception(GetTypeNotRealExcMsg(type));
            if (!type.IsInheritorOf(BindType)) throw new Exception(GetNotInheritorExcMsg(type, BindType));
        }

        private void ValidationToSingleton(Type type)
        {
            if (type == null) throw new Exception(GetTypeIsNullExcMsg());
            if (type.IsInterface || type.IsAbstract) throw new Exception(GetTypeNotRealExcMsg(type));
            if (!type.IsInheritorOf(BindType)) throw new Exception(GetNotInheritorExcMsg(type, BindType));
        }

        private void ValidationToInstance(Type type, object instance)
        {
            if (type == null) throw new Exception(GetTypeIsNullExcMsg());
            if (instance == null) throw new Exception(GetInstanceIsNullExcMsg());
            if (!type.IsInheritorOf(BindType)) throw new Exception(GetNotInheritorExcMsg(type, BindType));
            if (!instance.GetType().IsInheritorOf(type)) throw new Exception(GetNotInheritorExcMsg(instance.GetType(), type));
        }

        private static string GetInstanceIsNullExcMsg()
        {
            return "Instance object is null";
        }
        
        private static string GetTypeIsNullExcMsg()
        {
            return "Type is null";
        }

        private static string GetNotInheritorExcMsg(Type child, Type parent)
        {
            return TypeExt.GetNotInheritorExcMsg(child, parent);
        }
        
        private static string GetTypeNotRealExcMsg(Type type)
        {
            return $"\"{type.FullName}\"can not be interface or abstract clss";
        }
    }

    public abstract class CucuFactoryBase : IObjectFactory
    {
        public Type InstanceType { get; }
        
        public CucuFactoryBase(Type instanceType)
        {
            InstanceType = instanceType;
        }

        public abstract object Get();

        public T Get<T>()
        {
            if (!typeof(T).IsInheritorOf(InstanceType))
                throw new Exception(TypeExt.GetNotInheritorExcMsg(typeof(T), InstanceType));
            
            var get = Get();

            if (get == null) return default;
            
            if (get is T t) return t;

            throw new Exception($"Can not cast \"{get.GetType().FullName}\" to \"{typeof(T).FullName}\"");
        }
    }

    public class CucuFactoryCreate : CucuFactoryBase
    {
        public CucuFactoryCreate(Type instanceType) : base(instanceType)
        {
        }

        public override object Get()
        {
            return ObjectCreator.Instance.Create(InstanceType);
        }
    }

    public class CucuFactoryInstance : CucuFactoryBase
    {
        private object _instance;

        public CucuFactoryInstance(Type instanceType, object instance) : base(instanceType)
        {
            _instance = instance;
        }

        public override object Get()
        {
            return _instance;
        }
    }

    public static class TypeExt
    {
        public static bool IsInheritorOf(this Type a, Type b, bool includeEquals = true)
        {
            if (includeEquals && a == b) return true;
            
            return b.IsInterface ? a.GetInterfaces().Contains(b) : a.IsSubclassOf(b);
        }

        public static string GetNotInheritorExcMsg(Type child, Type parent)
        {
            return $"\"{child.FullName}\" is not inheritor of \"{parent.FullName}\"";
        }
    }
}