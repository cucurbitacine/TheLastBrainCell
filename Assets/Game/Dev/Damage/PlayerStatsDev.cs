using Game.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterController = Game.Characters.CharacterController;

namespace Game.Dev.Damage
{
    public class PlayerStatsDev : MonoBehaviour
    {
        public CharacterController character;
        
        [Space]
        public Image healthBar;
        public TextMeshProUGUI healthText;
        public Image staminaBar;
        public TextMeshProUGUI staminaText;
        
        public float damp = 4f;

        private float _healthFillAmount = 0f;
        private float _staminaFillAmount = 0f;

        private void HealthUpdate()
        {
            _healthFillAmount = (float)character.Health.Value / character.Health.MaxValue;

            healthText.text = $"{character.Health.Value} / {character.Health.MaxValue}";
        }

        private void StaminaUpdate()
        {
            _staminaFillAmount = (float)character.Stamina.Value / character.Stamina.MaxValue;
            
            staminaText.text = $"{character.Stamina.Value / 10} / {character.Stamina.MaxValue / 10}";
        }

        private void HealthChanged(int value)
        {
            HealthUpdate();
        }

        private void StaminaChanged(int value)
        {
            StaminaUpdate();
        }

        private void Awake()
        {
            if (character == null) character = FindObjectOfType<PlayerController>();
        }

        private void OnEnable()
        {
            character.Health.Events.OnValueChanged.AddListener(HealthChanged);
            character.Stamina.Events.OnValueChanged.AddListener(StaminaChanged);
        }

        private void Start()
        {
            HealthUpdate();
            StaminaUpdate();
        }

        private void Update()
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, _healthFillAmount, damp * Time.deltaTime);
            staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, _staminaFillAmount, damp * Time.deltaTime);
        }

        private void OnDisable()
        {
            character.Health.Events.OnValueChanged.RemoveListener(HealthChanged);
            character.Stamina.Events.OnValueChanged.RemoveListener(StaminaChanged);
        }
    }
}