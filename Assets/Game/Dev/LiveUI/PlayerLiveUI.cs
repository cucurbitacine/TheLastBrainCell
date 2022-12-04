using System;
using Game.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Dev.LiveUI
{
    public class PlayerLiveUI : MonoBehaviour
    {
        public CharacterControllerBase character;

        public Image s1;
        public Image s2;
        public Image s3;

        private void Update()
        {
            s1.enabled = character.Stamina.Value > 0;
            s2.enabled = character.Stamina.Value > 1;
            s3.enabled = character.Stamina.Value > 2;
        }
    }
}
