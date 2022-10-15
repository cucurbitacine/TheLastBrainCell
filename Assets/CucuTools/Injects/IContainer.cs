using System;

namespace CucuTools.Injects
{
    public interface IContainer : ICanBinding, ICanResolving
    {
        Guid Guid { get; }
        string Key { get; }
    }

    public interface ICanBinding
    {
        Type[] GetBindTypes();
        
        IBinder Bind(Type type);
        IBinder Bind<T>();
        
        void Unbind(Type type);
        void Unbind<T>();
    }

    public interface ICanResolving
    {
        bool TryResolve(Type type, out object result);
        bool TryResolve<T>(out T result);
        
        object Resolve(Type type);
        T Resolve<T>();
    }
    
    public interface IBinder
    {
        Type BindType { get; }
        IObjectFactory Factory { get; }
        
        void To(Type type);
        void To<T>();

        void ToSingleton(Type type);
        void ToSingleton<T>();

        void ToInstance(Type type, object instance);
        void ToInstance<T>(T instance);

        void ToSelf();
    }

    public interface IObjectFactory
    {
        Type InstanceType { get; }
        
        object Get();
        T Get<T>();
    }
}