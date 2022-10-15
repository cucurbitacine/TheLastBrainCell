using System;

namespace CucuTools.Scenes
{
    /// <summary>
    /// Attribute for class of scene controller.
    /// Set scene name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CucuSceneControllerAttribute : Attribute
    {
        public string SceneName { get; }
        
        public CucuSceneControllerAttribute(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}