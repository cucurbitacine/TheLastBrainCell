using System;
using System.Threading.Tasks;
using Game.Effects;
using Game.Levels;
using UnityEngine;

namespace Game.Dev
{
    public class SplashScreenController : MonoBehaviour
    {
        public BaseFx startGame;
        public BaseFx winGame;
        public BaseFx loseGame;

        [Space]
        public GameSceneController gameScene;

        private async void DelayedStart()
        {
            await Task.Delay(1000);

            startGame.Play();
        }
        
        private void Awake()
        {
            if (gameScene == null) gameScene = FindObjectOfType<GameSceneController>();
        }

        private void OnEnable()
        {
            gameScene.events.onGameStart.AddListener(DelayedStart);
            gameScene.events.onGameWin.AddListener(winGame.Play);
            gameScene.events.onGameLose.AddListener(loseGame.Play);
        }
    }
}