using System;
using System.Threading.Tasks;
using Cinemachine;
using CucuTools.Attributes;
using CucuTools.Injects;
using CucuTools.Scenes;
using Game.Characters.Player;
using Game.Scores;
using Game.Scores.Handlers;
using Game.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Levels
{
    [CucuSceneController("Dev_Base")]
    public class GameSceneController : CucuSceneController
    {
        public bool playAgainAfterDeath = true;
        
        [Space]
        [CucuArg] public GameArg gameArg;

        [Space]
        public PlayerController player;
        public PlayerController playerPrefab;
        public Transform playerSpawnPosition;

        [Space]
        public CinemachineVirtualCamera cameraFollower;
        public AllNpcDead allNpcDead;
        public TimerController timerController;
        public ScoreTimeHandler scoreTime;
        public ScoreManager scoreManager;
        
        [Space]
        public UnityEvent<PlayerController> onPlayerSpawned;
        public UnityEvent<PlayerController> onPlayerDespawned;

        #region Public API

        [CucuButton("Start Game")]
        public void StartGame()
        {
            allNpcDead.onAllDead.AddListener(OnPlayerWin);
            
            scoreManager.ClearScore();
            timerController.StartTimer();
            
            SpawnPlayer();
        }
        
        [CucuButton("Stop Game")]
        public void StopGame()
        {
            allNpcDead.onAllDead.RemoveListener(OnPlayerWin);
            
            timerController.StopTimer();
        }
        
        [CucuButton("Play Again")]
        public void PlayAgain()
        {
            var loadingArg = new LoadingSceneArg(CucuSceneManager.GetSceneName<GameSceneController>(), gameArg);
            var scoreArg = new ScoreInfoArg();
            scoreArg.lastScore = scoreManager.totalScore;
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg, scoreArg);
        }
        
        [CucuButton("Return To Menu")]
        public void ReturnToMenu()
        {
            var loadingArg = new LoadingSceneArg(gameArg.menuSceneName);
            
            CucuSceneManager.LoadSingleScene<LoadingSceneController>(loadingArg);
        }

        [CucuButton("Clear Best Score")]
        public void ClearBestScore()
        {
            var key = ScoreInfoArg.BestScoreKeyName;

            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }
        
        #endregion

        #region Private API

        private void SpawnPlayer()
        {
            if (player == null)
            {
                player = Instantiate(playerPrefab, playerSpawnPosition.position, playerSpawnPosition.rotation);
            }
            
            cameraFollower.Follow = player.transform;
            
            onPlayerSpawned.Invoke(player);
            
            player.Health.Events.OnValueIsEmpty.AddListener(OnPlayerDead);
        }
        
        private void DespawnPlayer()
        {
            if (player == null) return;
            
            cameraFollower.Follow = null;
            
            onPlayerDespawned.Invoke(player);
            
            Destroy(player.gameObject);

            player = null;
        }

        private async void OnPlayerWin()
        {
            await Task.Delay(1000);
            
            StopGame();
            
            scoreTime.Score();
            
            UpdateBestScore();
            
            await Task.Delay(1000);
            
            PlayAgain();
        }
        
        private async void OnPlayerDead()
        {
            StopGame();
            
            UpdateBestScore();
            
            player.Health.Events.OnValueIsEmpty.RemoveListener(OnPlayerDead);
            
            DespawnPlayer();

            await Task.Delay(1000);
            
            if (playAgainAfterDeath) PlayAgain();
            else SpawnPlayer();
        }

        private void UpdateBestScore()
        {
            var key = ScoreInfoArg.BestScoreKeyName;
            if (PlayerPrefs.HasKey(key))
            {
                var bestScore = PlayerPrefs.GetInt(key);
                var totalScore = scoreManager.totalScore;
                if (totalScore > bestScore)
                {
                    PlayerPrefs.SetInt(key, totalScore);
                }
            }
            else
            {
                PlayerPrefs.SetInt(key, scoreManager.totalScore);
            }
        }
        
        #endregion

        protected override void OnAwake()
        {
            base.OnAwake();

            if (scoreManager == null) scoreManager = ScoreManager.Instance;
            if (allNpcDead == null) allNpcDead = FindObjectOfType<AllNpcDead>();
            if (timerController == null) timerController = FindObjectOfType<TimerController>();
            if (scoreTime == null) scoreTime = FindObjectOfType<ScoreTimeHandler>();
        }

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