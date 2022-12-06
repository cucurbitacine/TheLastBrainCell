using UnityEngine;

namespace Game.Tools
{
    public class TimerController : MonoBehaviour
    {
        public float seconds;
        public bool isActive;
        
        public void StartTimer()
        {
            isActive = true;
            seconds = 0f;
        }
        
        public void StopTimer()
        {
            isActive = false;
        }

        private void UpdateTimer(float deltaTime)
        {
            seconds += deltaTime;
        }

        private void Update()
        {
            if (isActive) UpdateTimer(Time.deltaTime);
        }
    }
}