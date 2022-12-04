using UnityEngine;

namespace Game.Effects
{
    [CreateAssetMenu(menuName = menuName, fileName = fileName, order = 0)]
    public class FxTemplate : ScriptableObject
    {
        public const string menuName = GameManager.title + "/" + BaseFx.title + "/Create " + fileName;
        public const string fileName = nameof(FxTemplate);
        
        public BaseFx fxPrefab;

        [Space]
        public bool needParent;
    }
}