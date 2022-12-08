using System.Collections;
using Game.Effects;
using TMPro;
using UnityEngine;

namespace Game.Dev
{
    public class SplashScreenTextFx : BaseFx
    {
        [Min(0f)]
        public float duration = 3f;
        
        [Space]
        public TextMeshProUGUI text;

        private Coroutine _playing;
        
        public override bool isPlaying => text.enabled;
        
        public override void Play()
        {
            if (_playing != null) StopCoroutine(_playing);
            _playing = StartCoroutine(Playing());
            
            text.enabled = true;
        }

        public override void Stop()
        {
            text.enabled = false;
        }

        private IEnumerator Playing()
        {
            yield return new WaitForSeconds(duration);
            
            Stop();
        }
        
        private void Awake()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
        }
    }
}
