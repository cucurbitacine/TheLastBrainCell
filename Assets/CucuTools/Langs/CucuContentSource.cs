using System.Linq;
using UnityEngine;

namespace CucuTools.Langs
{
    public abstract class CucuContentSource : ScriptableObject
    {
        public const string Root = Cucu.CreateAsset + "Langs/";
        public const string CreateAsset = Root + "Content Sources/";
    }
    
    public abstract class CucuContentSource<TContent> : CucuContentSource
    {
        public abstract TContent Content { get; }
    }
    
    public abstract class LangContentSource<TContent> : CucuContentSource<TContent>
    {
        public override TContent Content => Cells.FirstOrDefault(c => c.Lang == CucuLangManager.CurrentLang).Content;

        public LangContent<TContent>[] Cells;
    }
}