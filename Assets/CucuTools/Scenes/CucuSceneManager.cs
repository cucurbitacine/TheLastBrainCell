using System;
using System.Linq;
using System.Reflection;
using CucuTools.Injects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CucuTools.Scenes
{
    /// <summary>
    /// Wrapper around <see cref="UnityEngine.SceneManagement.SceneManager"/> only with setting <see cref="CucuArg"/> by <see cref="CucuArgumentManager"/>
    /// </summary>
    public static class CucuSceneManager
    {
        private static CucuArgumentManager CucuArgumentManager => CucuArgumentManager.Singleton;

        public static Scene GetActiveScene() => SceneManager.GetActiveScene();
        
        #region Unload Async

        public static AsyncOperation UnloadSceneAsync(string name)
        {
            return SceneManager.UnloadSceneAsync(name);
        }

        public static AsyncOperation UnloadSceneAsync<TController>()
            where TController : CucuSceneController
        {
            if (TryGetSceneName<TController>(out var name, out var msg))
                return SceneManager.UnloadSceneAsync(name);
            else
                throw new Exception($"Unload scene of \"{typeof(TController).Name}\" was failed :: {msg}");
        }
        
        #endregion
        
        #region Load Async

        /// <summary>
        /// Async load scene <param name="name"></param> with mode <param name="mode"></param>
        /// </summary>
        /// <param name="name">Scene name</param>
        /// <param name="mode">Load mode</param>
        public static AsyncOperation LoadSceneAsync(string name, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(name, mode);
        }
        
        public static AsyncOperation LoadSceneAsync(string name, LoadSceneMode mode, params CucuArg[] args)
        {
            CucuArgumentManager.SetArguments(args);
            
            return LoadSceneAsync(name, mode);
        }
        
        public static AsyncOperation LoadSingleSceneAsync(string name, params CucuArg[] args)
        {
            return LoadSceneAsync(name, LoadSceneMode.Single, args);
        }

        public static AsyncOperation LoadAdditiveSceneAsync(string name, params CucuArg[] args)
        {
            return LoadSceneAsync(name, LoadSceneMode.Additive, args);
        }
        
        public static AsyncOperation LoadSceneAsync<TController>(LoadSceneMode mode, params CucuArg[] args)
            where TController : CucuSceneController
        {
            if (TryGetSceneName<TController>(out var name, out var msg))
                return LoadSceneAsync(name, mode, args);
            
            throw new Exception($"Load scene of \"{typeof(TController).Name}\" was failed :: {msg}");
        }

        public static AsyncOperation LoadSingleSceneAsync<TController>(params CucuArg[] args)
            where TController : CucuSceneController
        {
            return LoadSceneAsync<TController>(LoadSceneMode.Single, args);
        }
        
        public static AsyncOperation LoadAdditiveSceneAsync<TController>(params CucuArg[] args)
            where TController : CucuSceneController
        {
            return LoadSceneAsync<TController>(LoadSceneMode.Additive, args);
        }
        
        #endregion

        #region Load

        /// <summary>
        /// Load scene <param name="name"></param> with mode <param name="mode"></param>
        /// </summary>
        /// <param name="name">Scene name</param>
        /// <param name="mode">Load mode</param>
        public static void LoadScene(string name, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(name, mode);
        }
        
        public static void LoadScene(string name, LoadSceneMode mode, params CucuArg[] args)
        {
            CucuArgumentManager.SetArguments(args);

            LoadScene(name, mode);
        }

        public static void LoadSingleScene(string name, params CucuArg[] args)
        {
            LoadScene(name, LoadSceneMode.Single, args);
        }
        
        public static void LoadAdditiveScene(string name, params CucuArg[] args)
        {
            LoadScene(name, LoadSceneMode.Additive, args);
        }

        public static void LoadScene<TController>(LoadSceneMode mode, params CucuArg[] args)
            where TController : CucuSceneController
        {
            if (TryGetSceneName<TController>(out var name, out var msg))
                LoadScene(name, mode, args);
            
            throw new Exception($"Load scene of \"{nameof(TController)}\" was failed :: {msg}");
        }
        
        public static void LoadSingleScene<TController>(params CucuArg[] args)
            where TController : CucuSceneController
        {
            LoadScene<TController>(LoadSceneMode.Single, args);
        }
        
        public static void LoadAdditiveScene<TController>(params CucuArg[] args)
            where TController : CucuSceneController
        {
            LoadScene<TController>(LoadSceneMode.Additive, args);
        }
        
        #endregion

        /// <summary>
        /// Trying get scene name from attribute of scene controller
        /// </summary>
        /// <param name="sceneName">Result scene name</param>
        /// <param name="msg">Message</param>
        /// <typeparam name="TController">Type of scene controller</typeparam>
        /// <returns>Success</returns>
        public static bool TryGetSceneName<TController>(out string sceneName, out string msg) where TController : CucuSceneController
        {
            sceneName = null;
            msg = "";
            
            var attribute = (CucuSceneControllerAttribute) typeof(TController).GetCustomAttribute(typeof(CucuSceneControllerAttribute));

            if (attribute == null)
            {
                msg = $"{nameof(CucuSceneControllerAttribute)} was not found in custom attributes";
                return false;
            }
            
            sceneName = attribute.SceneName;

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                msg = $"Scene name is null or white space";
                return false;
            }
            
            return true;
        }
    }
}