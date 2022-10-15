namespace CucuTools.Langs
{
    public abstract class CucuLangBehaviour : CucuBehaviour
    {
        public abstract void UpdateLang();

        protected virtual void Awake()
        {
            CucuLangManager.AddListener(UpdateLang);
        }

        protected virtual void OnEnable()
        {
            UpdateLang();
        }

        protected virtual void OnValidate()
        {
            UpdateLang();
        }
    }
}