using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CucuTools.Injects;
using UnityEngine;

namespace CucuTools.Scenes
{
    public class CucuSceneQueueManager
    {
        public static CucuSceneQueueManager Global { get; }

        static CucuSceneQueueManager()
        {
            Global = new CucuSceneQueueManager();
        }
        
        protected ConcurrentQueue<SceneLoadParam> sceneQueue => _sceneQueue ?? (_sceneQueue = new ConcurrentQueue<SceneLoadParam>());
        private ConcurrentQueue<SceneLoadParam> _sceneQueue;

        public readonly List<string> ComplitedScenes = new List<string>(); 
        
        public void Add(params SceneLoadParam[] sceneInfos)
        {
            foreach (var sceneInfo in sceneInfos)
                sceneQueue.Enqueue(sceneInfo);
        }

        public void Add(string sceneName, params CucuArg[] args)
        {
            Add(new SceneLoadParam(sceneName, args));
        }
        
        public void Add<T>(params CucuArg[] args) where T : CucuSceneController
        {
            if (CucuSceneManager.TryGetSceneName<T>(out var scene, out _))
                Add(new SceneLoadParam(scene, args));
        }

        public int GetSceneCount()
        {
            return sceneQueue.Count;
        }

        public bool TryGetNextScene(out SceneLoadParam sceneLoadParam)
        {
            return sceneQueue.TryPeek(out sceneLoadParam);
        }

        public AsyncOperation TryLoadNextScene(out SceneLoadParam sceneLoadParam)
        {
            if (!sceneQueue.TryDequeue(out sceneLoadParam)) return null;

            return CucuSceneManager.LoadSingleSceneAsync(sceneLoadParam.sceneName, sceneLoadParam.args);
        }
    }
    
    [Serializable]
    public class SceneLoadParam
    {
        public string sceneName;
        public CucuArg[] args;
            
        public SceneLoadParam(string sceneName, params CucuArg[] args)
        {
            this.sceneName = sceneName;
            this.args = args ?? new CucuArg[0];
        }

        public SceneLoadParam(string sceneName) : this(sceneName, new CucuArg[0])
        {
        }

        public SceneLoadParam(SceneLoadParam sceneLoadParam) : this(sceneLoadParam.sceneName, sceneLoadParam.args)
        {
        }
    }
}