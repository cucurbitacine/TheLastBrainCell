using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CucuTools.Async
{
    public static class CucuCoroutine
    {
        public static MonoBehaviour Root
        {
            get
            {
                if (root != null) return root;
                
                var roots = Object.FindObjectsOfType<CucuCoroutineRoot>();
                root = roots.FirstOrDefault();
                for (int i = 0; i < roots.Length; i++)
                {
                    if (roots[i] == root) continue;
                    Destroy(roots[i]);
                    Destroy(roots[i].gameObject);
                }

                if (root != null)
                {
                    DontDestroyOnLoad(root.gameObject);
                    return root;
                }
                
                root = new GameObject(nameof(CucuCoroutineRoot)).AddComponent<CucuCoroutineRoot>();
                DontDestroyOnLoad(root.gameObject);
                
                return root;
            }
        }

        private static CucuCoroutineRoot root;

        public static Coroutine Start(IEnumerator routine)
        {
            return Root.StartCoroutine(routine);
        }

        public static void Stop(Coroutine coroutine)
        {
            Root.StopCoroutine(coroutine);
        }

        public static void StopAll()
        {
            Root.StopAllCoroutines();
        }

        private static void Destroy(GameObject gameObject)
        {
            if (Application.isPlaying) Object.Destroy(gameObject);
            else Object.DestroyImmediate(gameObject);
        }
        
        private static void Destroy<T>(T component) where T : Component
        {
            if (Application.isPlaying) Object.Destroy(component);
            else Object.DestroyImmediate(component);
        }
        
        private static void DontDestroyOnLoad(GameObject gameObject)
        {
            gameObject.transform.SetParent(null);
            if (Application.isPlaying) Object.DontDestroyOnLoad(gameObject);
        } 
    }
}