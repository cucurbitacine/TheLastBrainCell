using TMPro;
using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionView : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        
        public void UpdateView()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
            _text.text = Application.version;
        }
        
        private void OnEnable()
        {
            UpdateView();
        }

        private void OnValidate()
        {
            UpdateView();
        }
    }
}
