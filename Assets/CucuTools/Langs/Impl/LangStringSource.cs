using UnityEngine;

namespace CucuTools.Langs.Impl
{
    [CreateAssetMenu(menuName = CucuContentSource.CreateAsset + AssetName, fileName = AssetName, order = 0)]
    public class LangStringSource : LangContentSource<string>
    {
        public const string AssetName = nameof(LangStringSource);
    }
}