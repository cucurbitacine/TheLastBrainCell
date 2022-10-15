using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CucuTools.Langs.Impl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class CucuLangImageUI : CucuLangBehaviour
    {
        private Image imageUI;

        #region SerializeField

        [CucuReadOnly]
        [SerializeField] private bool isValid;
        [SerializeField] private CucuContentSource<Sprite> contentSource;

        #endregion

        #region Properties

        public CucuContentSource<Sprite> ContentSource => contentSource;
        public Image ImageUI => imageUI != null ? imageUI : (imageUI = GetComponent<Image>());
        
        public bool IsValid
        {
            get => isValid;
            private set => isValid = value;
        }

        #endregion

        public override void UpdateLang()
        {
            if (IsValid) ImageUI.sprite = ContentSource.Content;
        }

        private void Validate()
        {
            IsValid = ImageUI != null && ContentSource != null;
        }

        private void Start()
        {
            Validate();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            Validate();
        }
    }
}