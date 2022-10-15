using System;
using CucuTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CucuTools.Blend
{
    [AddComponentMenu(Cucu.AddComponent + Cucu.BlendGroup + ObjectName, 0)]
    public class CucuTimer : CucuBehaviour
    {
        public const string ObjectName = "Timer";
        
        #region SerializeField

        [CucuReadOnly]
        [SerializeField] private bool isPlaying;
        [SerializeField] private bool paused;
        [SerializeField] private bool autoStart;
        [SerializeField] private TimerInfo info;
        [SerializeField] private TimerEvents events;

        #endregion
        
        public bool IsPlaying
        {
            get => isPlaying;
            private set => isPlaying = value;
        }
        
        public bool Paused
        {
            get => paused;
            set => paused = value;
        }
        
        public TimerInfo Info => info ?? (info = new TimerInfo());
        public TimerEvents Events => events ?? (events = new TimerEvents());

        [CucuButton("Start", group: "Timer")]
        public void StartTimer()
        {
            if (!Application.isPlaying) return;
            
            if (IsPlaying) return;

            IsPlaying = true;
            Paused = false;
            Info.StartTimer();
            
            Events.OnStartTimer.Invoke();
            Events.OnProgressChanged.Invoke(Info.Progress);
        }

        [CucuButton("Stop", group: "Timer")]
        public void StopTimer()
        {
            if (!Application.isPlaying) return;
            
            if (!IsPlaying) return;

            IsPlaying = false;
            Paused = false;
            Info.StopTimer();
            
            Events.OnProgressChanged.Invoke(Info.Progress);
            Events.OnStopTimer.Invoke();
        }

        private void UpdateTimer(float deltaTime)
        {
            if (Info.NeedStop())
            {
                StopTimer();
                return;
            }

            Info.DeltaTime(deltaTime);

            Events.OnProgressChanged.Invoke(Info.Progress);
        }

        private void Start()
        {
            if (autoStart) StartTimer();
        }

        private void Update()
        {
            if (IsPlaying && !Paused) UpdateTimer(Time.deltaTime);
        }
    }

    [Serializable]
    public class TimerInfo
    {
        public float Timer
        {
            get => timer;
            private set => timer = Mathf.Clamp(value, 0f, Duration);
        }

        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0, value);
        }
        
        public float Duration
        {
            get => duration;
            set => duration = Mathf.Max(0, value);
        }

        public TimerMode Mode
        {
            get => mode;
            set => mode = value;
        }
        
        [CucuReadOnly]
        [SerializeField] private float timer = 0f;
        [Min(0)]
        [SerializeField] private float speed = 1f;
        [Min(0)]
        [SerializeField] private float duration = 1f;
        [SerializeField] private TimerMode mode;

        private bool reverse;
        
        public TimerInfo(float duration, float speed)
        {
            Timer = 0f;
            Duration = duration;
            Speed = speed;
        }

        public TimerInfo(float duration) : this(duration, 1f)
        {
        }

        public TimerInfo() : this(1f)
        {
        }
        
        public float Progress => Duration > 0f ? Timer / Duration : 1f;
        
        public void StartTimer()
        {
            Timer = 0;
            reverse = false;
        }
        
        public void StopTimer()
        {
            Timer = Duration;
        }
        
        public bool NeedStop()
        {
            return Mode == TimerMode.OneShot && Timer >= Duration;
        }

        public void DeltaTime(float dt)
        {
            if (Mode == TimerMode.OneShot) Timer += dt * Speed;
            if (Mode == TimerMode.Repeat) Timer = Mathf.Repeat(Timer + dt * Speed, Duration);
            if (Mode == TimerMode.PingPong)
            {
                Timer += (reverse ? -1 : 1) * dt * Speed;
                if (reverse)
                {
                    if (Timer <= 0) reverse = false;
                }
                else
                {
                    if (Timer >= Duration) reverse = true;
                }
            }
        }

        public enum TimerMode
        {
            OneShot,
            Repeat,
            PingPong,
        }
    }

    [Serializable]
    public class TimerEvents
    {
        public UnityEvent OnStartTimer => onStartTimer ?? (onStartTimer = new UnityEvent());
        public UnityEvent OnStopTimer => onStopTimer ?? (onStopTimer = new UnityEvent());
        public UnityEvent<float> OnProgressChanged => onProgressChanged ?? (onProgressChanged = new UnityEvent<float>());
        
        [SerializeField] private UnityEvent onStartTimer;
        [SerializeField] private UnityEvent onStopTimer;
        [SerializeField] private UnityEvent<float> onProgressChanged;
    }
}