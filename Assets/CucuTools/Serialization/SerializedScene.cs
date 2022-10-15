using System;
using UnityEngine;

namespace CucuTools.Serialization
{
    [Serializable]
    public class SerializedScene
    {
        public string SceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }
        
        public SerializedGameObject[] GameObjects
        {
            get => _gameObjects;
            set => _gameObjects = value;
        }
        
        [SerializeField] private string _sceneName;
        [SerializeField] private SerializedGameObject[] _gameObjects;

        public SerializedScene(string sceneName, params SerializedGameObject[] gameObjects)
        {
            SceneName = sceneName;
            GameObjects = gameObjects;
        }
    }
    
    [Serializable]
    public class SerializedGameObject
    {
        public Guid Guid
        {
            get => Guid.TryParse(_guid, out var guid) ? guid : Guid.Empty;
            set => _guid = value.ToString();
        }
        
        public SerializedComponent[] Components
        {
            get => _components;
            set => _components = value;
        }
        
        [SerializeField] private string _guid;
        [SerializeField] private SerializedComponent[] _components;

        public SerializedGameObject(Guid guid, params SerializedComponent[] components)
        {
            Guid = guid;
            Components = components;
        }
    }
    
    
    [Serializable]
    public class SerializedComponent
    {
        public string TypeName 
        {
            get => _typeName;
            set => _typeName = value;
        }
        
        public byte[] Bytes
        {
            get => _bytes;
            set => _bytes = value;
        }
        
        [SerializeField] private string _typeName;
        [SerializeField] private byte[] _bytes;

        public SerializedComponent(string typeName, byte[] bytes)
        {
            TypeName = typeName;
            Bytes = bytes;
        }
        
        public SerializedComponent(Type type, byte[] bytes) : this(type.FullName, bytes)
        {
        }
    }
}