using UnityEngine.Events;

namespace CucuTools.Langs
{
    public enum LangType
    {
        Eng,
        Rus,
    }

    public static class CucuLangManager
    {
        public static LangType CurrentLang
        {
            get => _currentLang;
            set
            {
                if (value == _currentLang) return;
                _currentLang = value;
                OnLangChange.Invoke();
            }
        }

        private static LangType _currentLang;
        private static UnityEvent OnLangChange { get; } = new UnityEvent();

        public static void AddListener(UnityAction unityAction)
        {
            OnLangChange.AddListener(unityAction);
        }
        
        public static void RemoveListener(UnityAction unityAction)
        {
            OnLangChange.AddListener(unityAction);
        }
    }
}