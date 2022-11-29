using System;
using Game.Inputs;
using Game.Inputs.Combos;
using Game.Levels;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Game.Characters.Player
{
    public class ComboStatusDisplay : MonoBehaviour
    {
        public PositionConstraint positionConstraint;
        public Slider slider;
        public Image fillImage;
        public Image readyImage;
        public PlayerInputController playerInput;

        public ComboController ComboController => playerInput.comboController;

        private GameSceneController gameScene;

        private void OnPlayerSpawned(PlayerController player)
        {
            playerInput = player.GetComponentInChildren<PlayerInputController>();
            positionConstraint.SetSource(0, new ConstraintSource() { sourceTransform = player.transform, weight = 1f });
        }

        private void OnEnable()
        {
            gameScene = FindObjectOfType<GameSceneController>();

            if (gameScene != null)
            {
                gameScene.onPlayerSpawned.AddListener(OnPlayerSpawned);
            }
        }

        private void OnDisable()
        {
            if (gameScene != null)
            {
                gameScene.onPlayerSpawned.RemoveListener(OnPlayerSpawned);
            }
        }

        private void Update()
        {
            slider.enabled = ComboController.numberAttack >= 0;
            slider.gameObject.SetActive(slider.enabled);
            
            var haveStamina = !playerInput.Character.AttackSetting.useStamina ||
                              playerInput.Character.Stamina.Value >=
                              playerInput.Character.AttackSetting.staminaCost;
            
            readyImage.gameObject.SetActive(haveStamina);
            
            if (slider.enabled)
            {
                slider.value = ComboController.timer / ComboController.combo.attacks[ComboController.numberAttack].duration;

                var canAttack = ComboController.IsTimerValid() && haveStamina;
                
                readyImage.gameObject.SetActive(canAttack);
                
                fillImage.color = canAttack ? Color.green : Color.red;
            }
        }
    }
}
