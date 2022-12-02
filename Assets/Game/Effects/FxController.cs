using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Effects
{
    public class FxController : MonoBehaviour
    {
        private static FxController _instance = null;

        public static FxController Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var fxControllers = FindObjectsOfType<FxController>();

                for (var i = 0; i < fxControllers.Length; i++)
                {
                    if (fxControllers[i].isSingleton)
                    {
                        _instance = fxControllers[i];
                        return _instance;
                    }
                }
                
                _instance = new GameObject(nameof(FxController)).AddComponent<FxController>();
                
                DontDestroyOnLoad(_instance.gameObject);
                _instance.isSingleton = true;
                
                return _instance;
            }
        }

        public bool isSingleton = false;
        
        private readonly Dictionary<BaseFx, List<BaseFx>> _vfxStorage = new Dictionary<BaseFx, List<BaseFx>>();

        public void Play(FxTemplate template, Vector2? pos = null, Quaternion? rot = null)
        {
            if (template.fxPrefab == null) return;
            
            if (!_vfxStorage.TryGetValue(template.fxPrefab, out var fxList))
            {
                fxList = new List<BaseFx>();
                _vfxStorage.Add(template.fxPrefab, fxList);
            }
            
            var fx = fxList.FirstOrDefault(v=>!v.isPlaying);

            if (fx == null)
            {
                if (template.needParent)
                {
                    fx = Instantiate(template.fxPrefab, transform);
                }
                else
                {
                    fx = Instantiate(template.fxPrefab);
                }
                
                fxList.Add(fx);
            }

            if (pos != null) fx.transform.position = pos.Value;
            if (rot != null) fx.transform.rotation = rot.Value;
            
            fx.Play();
        }
        
        private void Awake()
        {
            if (isSingleton)
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }
}