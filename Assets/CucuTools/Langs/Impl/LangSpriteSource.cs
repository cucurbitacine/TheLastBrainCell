using UnityEngine;

namespace CucuTools.Langs.Impl
{
    [CreateAssetMenu(menuName = CucuContentSource.CreateAsset + AssetName, fileName = AssetName, order = 0)]
    public class LangSpriteSource : LangContentSource<Sprite>
    {
        public const string AssetName = nameof(LangSpriteSource);
    }
}