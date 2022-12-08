using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev
{
    public class ParamView : MonoBehaviour
    {
        [Space]
        public string tittle = "Tittle";
        [Space]
        public ParamViewType viewType = ParamViewType.Int;
        public int minValue = 0;
        public int maxValue = 1;
        
        public float value
        {
            get => sliderView.value;
            set => sliderView.value = value;
        }
        
        [Space]
        public TextMeshProUGUI tittleView;
        public TextMeshProUGUI valueView;
        public Slider sliderView;

        public void UpdateView()
        {
            sliderView.SetValueWithoutNotify(Mathf.Clamp(value, minValue, maxValue));

            if (sliderView != null)
            {
                sliderView.wholeNumbers = viewType == ParamViewType.Int;
                sliderView.minValue = minValue;
                sliderView.maxValue = maxValue;
            }

            if (tittleView != null)
            {
                tittleView.text = tittle;
            }
            
            if (valueView != null)
            {
                valueView.text = viewType == ParamViewType.Int ? value.ToString("00") : value.ToString("F1");
            }
        }

        private void Awake()
        {
            sliderView.onValueChanged.AddListener(t => value = t);
        }

        private void Update()
        {
            UpdateView();
        }

        private void OnValidate()
        {
            UpdateView();
        }
    }

    public enum ParamViewType
    {
        Int,
        Float,
    }
}
