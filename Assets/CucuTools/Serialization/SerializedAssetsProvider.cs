using UnityEngine;

namespace CucuTools.Serialization
{
    [CreateAssetMenu(menuName = CreateAsset + AssetName, fileName = AssetName, order = 0)]
    public class SerializedAssetsProvider : ScriptableObject
    {
        public const string Root = "Serialization/";
        public const string CreateAsset = Cucu.CreateAsset + Root;
        public const string AssetName = nameof(SerializedAssetsProvider);
        
        public SerializedScene SerializedScene;

        public void SaveData()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}