using System;
using UnityEngine;

namespace CucuTools.Serialization
{
    /// <summary>
    /// Serializable Component Entity
    /// </summary>
    [RequireComponent(typeof(SerializableGameObject))]
    public abstract class SerializableComponent : CucuBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private GameObject gameObjectRef;
        [SerializeField] private Component componentRef;

        #region Public API

        /// <summary>
        /// Serialization Core
        /// </summary>
        public virtual Serializator Serializator { get; set; } = Serializator.Current;
        
        /// <summary>
        /// Do Serialization or not
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        /// <summary>
        /// Is Valid component
        /// </summary>
        public bool IsValid => ComponentRef != null;
        
        /// <summary>
        /// Reference to gameObject with component
        /// </summary>
        public GameObject GameObjectRef
        {
            get => gameObjectRef != null ? gameObjectRef : (gameObjectRef = gameObject);
            set
            {
                gameObjectRef = value;
                componentRef = GameObjectRef.GetComponent(ComponentType);
            }
        }

        /// <summary>
        /// Referenc to component
        /// </summary>
        public Component ComponentRef => componentRef != null ? componentRef : (componentRef = GameObjectRef.GetComponent(ComponentType));

        #endregion

        #region Abstract

        /// <summary>
        /// Component Type
        /// </summary>
        public abstract Type ComponentType { get; }
        
        /// <summary>
        /// Serialize component
        /// </summary>
        /// <returns></returns>
        public abstract SerializedComponent SerializeComponent();
        
        /// <summary>
        /// Deserialize component
        /// </summary>
        /// <param name="serializedComponent"></param>
        public abstract void DeserializeComponent(SerializedComponent serializedComponent);

        #endregion
    }

    /// <summary>
    /// Typed Serializable Component Entity
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class SerializableComponent<TComponent> : SerializableComponent where TComponent : Component
    {
        /// <summary>
        /// Component
        /// </summary>
        public TComponent Component => ComponentRef as TComponent;

        #region Override

        /// <inheritdoc />
        public sealed override Type ComponentType => typeof(TComponent);

        /// <inheritdoc />
        public sealed override SerializedComponent SerializeComponent()
        {
            return new SerializedComponent(ComponentType, Serialize());
        }

        /// <inheritdoc />
        public sealed override void DeserializeComponent(SerializedComponent serializedComponent)
        {
            Deserialize(serializedComponent.Bytes);
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Serialize
        /// </summary>
        /// <returns></returns>
        protected abstract byte[] Serialize();
        
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="bytes"></param>
        protected abstract void Deserialize(byte[] bytes);

        #endregion
    }
    
    /// <summary>
    /// Typed Serializable Component Entity with Serializable Data
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class SerializableComponent<TComponent, TData> : SerializableComponent<TComponent>
        where TComponent : Component
        where TData : class
    {
        #region Override

        /// <inheritdoc />
        protected sealed override byte[] Serialize()
        {
            return Serializator.Serialize<TData>(GetData());
        }

        /// <inheritdoc />
        protected sealed override void Deserialize(byte[] bytes)
        {
            SetData(Serializator.Deserialize<TData>(bytes));
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Get Serializable Data
        /// </summary>
        /// <returns></returns>
        public abstract TData GetData();
        
        /// <summary>
        /// Set Serializable Data
        /// </summary>
        /// <param name="data"></param>
        public abstract void SetData(TData data);

        #endregion
    }
}