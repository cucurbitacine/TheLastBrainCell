using System;
using UnityEngine;

namespace CucuTools.Langs
{
    [Serializable]
    public class LangContent<TContent>
    {
        [SerializeField] private TContent content;
        [SerializeField] private LangType lang;
        
        public TContent Content
        {
            get => content;
            set => content = value;
        }

        public LangType Lang
        {
            get => lang;
            set => lang = value;
        }
    }
}