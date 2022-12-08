using System;
using System.Collections.Generic;
using CucuTools;
using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scores
{
    public class ScoreManager : CucuBehaviour
    {
        private static ScoreManager _instance = null;

        public static ScoreManager Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var navigations = FindObjectsOfType<ScoreManager>();

                for (var i = 0; i < navigations.Length; i++)
                {
                    if (navigations[i].isSingleton)
                    {
                        _instance = navigations[i];
                        return _instance;
                    }
                }
                
                _instance = new GameObject(nameof(ScoreManager)).AddComponent<ScoreManager>();
                
                DontDestroyOnLoad(_instance.gameObject);
                _instance.isSingleton = true;
                
                return _instance;
            }
        }
        
        [CucuReadOnly]
        public int totalScore = 0;

        [Space]
        public bool isSingleton = true;

        [Space]
        public ScoreEvents @events = new ScoreEvents();
        
        [Space]
        public List<ScoreEvent> scores = new List<ScoreEvent>();
        
        public void AddScore(ScoreEvent e)
        {
            scores.Add(e);
            totalScore += e.score;
            
            events.onScoreAdded.Invoke(e);
            events.onScoreChanged.Invoke(totalScore);
        }

        [CucuButton("Clear Score")]
        public void ClearScore()
        {
            scores.Clear();
            totalScore = 0;
            
            events.onScoreChanged.Invoke(totalScore);
        }
        
        private void Awake()
        {
            if (isSingleton)
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (transform.parent != null) transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
        }
    }

    [Serializable]
    public class ScoreEvents
    {
        public UnityEvent<ScoreEvent> onScoreAdded = new UnityEvent<ScoreEvent>();
        public UnityEvent<int> onScoreChanged = new UnityEvent<int>();
    }
}