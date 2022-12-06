using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class TouchUIController : MonoBehaviour
    {
        public readonly int MoveX = Animator.StringToHash("MoveX");
        public readonly int MoveY = Animator.StringToHash("MoveY");
        
        public Image thumbstickPanel;
        public Animator thumbstickAnimator;
        
        [Space]
        public Button attackButton;
        public Button jumpButton;
    }
}
