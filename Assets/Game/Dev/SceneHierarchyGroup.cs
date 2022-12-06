using UnityEngine;

namespace Game.Dev
{
    public class SceneHierarchyGroup : MonoBehaviour
    {
        public string title = "Group";

        [Space]
        public string left = "____[";
        public string right = "]____";
        [Range(0, 32)]
        public int space = 12;

        public void UpdateGroupName()
        {
            var gameObjectName = "";

            for (var i = 0; i < space; i++)
            {
                gameObjectName += " ";
            }

            gameObjectName += title;
            
            for (var i = 0; i < space; i++)
            {
                gameObjectName += " ";
            }

            gameObjectName = $"{left}{gameObjectName}{right}";
            
            gameObject.name = gameObjectName;
        }

        private void OnValidate()
        {
            UpdateGroupName();
        }
    }
}
