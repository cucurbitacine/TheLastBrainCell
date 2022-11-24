using Game.Characters;
using Game.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterController = Game.Characters.CharacterController;

namespace Game.Dev.Damage
{
    public class PlayerStatsDev : MonoBehaviour
    {
        public CharacterController character;
        public GameSceneController gameSceneController;
        
        [Space]
        public Image healthBar;
        public TextMeshProUGUI healthText;
        public Image staminaBar;
        public TextMeshProUGUI staminaText;
        
        public float damp = 4f;

        private float _healthFillAmount = 0f;
        private float _staminaFillAmount = 0f;

        public void StatsUpdate()
        {
            if (character != null)
            {
                HealthUpdate();
                StaminaUpdate();
            }
        }
        
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

        private void EnablePlayer(CharacterController player)
        {
            character = player;
            
            character.Health.Events.OnValueChanged.AddListener(HealthChanged);
            character.Stamina.Events.OnValueChanged.AddListener(StaminaChanged);

            StatsUpdate();
        }
        
        private void DisablePlayer(CharacterController player)
        {
            if (character != player) return;
            
            character.Health.Events.OnValueChanged.RemoveListener(HealthChanged);
            character.Stamina.Events.OnValueChanged.RemoveListener(StaminaChanged);
            
            character = null; 
        }

        private void OnEnable()
        {
            if (character == null)
            {
                gameSceneController = FindObjectOfType<GameSceneController>();
                if (gameSceneController != null)
                {
                    gameSceneController.OnPlayerInitialized.AddListener(EnablePlayer);
                    gameSceneController.OnPlayerDeinitialized.AddListener(DisablePlayer);
                }
            }
            else
            {
                EnablePlayer(character);
            }
        }

        private void Start()
        {
            StatsUpdate();
        }

        private void Update()
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, _healthFillAmount, damp * Time.deltaTime);
            staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, _staminaFillAmount, damp * Time.deltaTime);
        }

        private void OnDisable()
        {
            if (gameSceneController != null)
            {
                gameSceneController.OnPlayerInitialized.RemoveListener(EnablePlayer);
                gameSceneController.OnPlayerDeinitialized.RemoveListener(DisablePlayer);
            }
            else
            {
                if (character != null) DisablePlayer(character);
            }
        }
    }
}