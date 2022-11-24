using System;
using System.Threading.Tasks;
using Cinemachine;
using CucuTools.Attributes;
using CucuTools.Injects;
using CucuTools.Scenes;
using Game.Characters;
using Game.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Levels
{
    [CucuSceneController("Dev_Base")]
    public class GameSceneController : CucuSceneController
    {
        [CucuArg] public GameArg gameArg;

        [Space]
        public PlayerController player;
        public PlayerController playerPrefab;
        public Transform playerSpawnPosition;

        [Space]
        public PlayerInputController playerInput;
        public CinemachineVirtualCamera cameraFollower;

        [Space]
        public UnityEvent<PlayerController> OnPlayerInitialized;
        public UnityEvent<PlayerController> OnPlayerDeinitialized;

        [CucuButton("Play Again")]
        public void PlayAgain()
        {
            var loadingArg = new LoadingSceneArg(CucuSceneManager.GetSceneName<GameSceneController>(), gameArg);
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }
        
        [CucuButton("Return To Menu")]
        public void ReturnToMenu()
        {
            var loadingArg = new LoadingSceneArg(gameArg.menuSceneName);
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }

        [CucuButton()]
        private void InitializePlayer()
        {
            player = Instantiate(playerPrefab, playerSpawnPosition.position, playerSpawnPosition.rotation);

            playerInput.Character = player;
            playerInput.enabled = true;

            cameraFollower.Follow = player.transform;
            
            OnPlayerInitialized.Invoke(player);
            
            player.Health.Events.OnValueIsEmpty.AddListener(OnPlayerDead);
        }
        
        [CucuButton()]
        private void DeinitializePlayer()
        {
            playerInput.enabled = false;
            playerInput.Character = null;
            
            cameraFollower.Follow = null;
            
            OnPlayerDeinitialized.Invoke(player);
            
            Destroy(player.gameObject);

            player = null;
        }
        
        private async void OnPlayerDead()
        {
            player.Health.Events.OnValueIsEmpty.RemoveListener(OnPlayerDead);
            
            DeinitializePlayer();

            await Task.Delay(1000);
            
            InitializePlayer();
        }

        private void Start()
        {
            InitializePlayer();
        }
        
        private void OnDrawGizmos()
        {
            if (playerSpawnPosition != null)
            {
                var spawnPosition = playerSpawnPosition.position;
                
                Gizmos.DrawWireSphere(spawnPosition, 0.1f);
                Gizmos.DrawLine(spawnPosition, spawnPosition + playerSpawnPosition.up);
            }
        }
    }
    
    [Serializable]
    public class GameArg : CucuArg
    {
        [Space]
        [CucuScene]
        public string menuSceneName;
    }
}