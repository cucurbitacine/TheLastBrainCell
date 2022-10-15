using System;
using UnityEngine;

namespace CucuTools.Injects
{
    /// <summary>
    /// Simple MonoBehaviour but with Inject on awake
    /// </summary>
    public abstract class InjectMonoBehaviour : CucuBehaviour
    {
        protected virtual IContainer Container { get; }
        
        private CucuArgumentManager CucuArgumentManager => CucuArgumentManager.Singleton;

        private void Inject()
        {
            try
            {
                Injector.InjectArgs(this, CucuArgumentManager.GetArgs());

                if (Container != null) Injector.InjectContainer(this, Container);
            }
            catch (Exception exc)
            {
                Debug.LogError($"Injection failed :: {exc}");
            }
        }
        
        protected abstract void OnAwake();
        
        protected virtual void Awake()
        {
            Inject();
            
            OnAwake();
        }
    }
}