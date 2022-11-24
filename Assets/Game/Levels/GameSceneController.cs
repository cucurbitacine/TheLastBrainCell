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
        public CinemachineVirtualCamera cameraFollower;

        [Space]
        public UnityEvent<PlayerController> onPlayerSpawned;
        public UnityEvent<PlayerController> onPlayerDespawned;

        #region Public API

        [CucuButton("StartGame")]
        public void StartGame()
        {
            SpawnPlayer();
        }
        
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

        #endregion

        #region Private API

        private void SpawnPlayer()
        {
            player = Instantiate(playerPrefab, playerSpawnPosition.position, playerSpawnPosition.rotation);

            cameraFollower.Follow = player.transform;
            
            onPlayerSpawned.Invoke(player);
            
            player.Health.Events.OnValueIsEmpty.AddListener(OnPlayerDead);
        }
        
        private void DespawnPlayer()
        {
            cameraFollower.Follow = null;
            
            onPlayerDespawned.Invoke(player);
            
            Destroy(player.gameObject);

            player = null;
        }
        
        private async void OnPlayerDead()
        {
            player.Health.Events.OnValueIsEmpty.RemoveListener(OnPlayerDead);
            
            DespawnPlayer();

            await Task.Delay(1000);
            
            SpawnPlayer();
        }

        #endregion

        private void Start()
        {
            StartGame();
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