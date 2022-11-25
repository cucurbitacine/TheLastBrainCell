using Game.Characters;
using Game.Levels;
using UnityEngine;

namespace Game.Dev.Damage
{
    public class CameraShakerOnDamage : MonoBehaviour
    {
        public PlayerController playerCurrent;

        [Space]
        public CameraShaker cameraShaker;
        public GameSceneController gameScene;

        private void Shake(int damage)
        {
            if(damage < 0) cameraShaker.Shake();
        }

        private void PlayerEnabled(PlayerController player)
        {
            if (playerCurrent != null) return;

            playerCurrent = player;

            player.Health.Events.OnValueChanged.AddListener(Shake);
        }

        private void PlayerDisabled(PlayerController player)
        {
            if (playerCurrent != player) return;

            playerCurrent.Health.Events.OnValueChanged.RemoveListener(Shake);

            playerCurrent = null;
        }

        private void Awake()
        {
            if (gameScene == null) gameScene = FindObjectOfType<GameSceneController>();
            if (cameraShaker == null) cameraShaker = GetComponent<CameraShaker>();
        }

        private void OnEnable()
        {
            gameScene.onPlayerSpawned.AddListener(PlayerEnabled);
            gameScene.onPlayerDespawned.AddListener(PlayerDisabled);
        }

        private void OnDisable()
        {
            gameScene.onPlayerSpawned.RemoveListener(PlayerEnabled);
            gameScene.onPlayerDespawned.RemoveListener(PlayerDisabled);
        }
    }
}