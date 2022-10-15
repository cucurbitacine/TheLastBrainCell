namespace CucuTools.IK
{
    public abstract class CucuIKObserver : CucuIKBehaviour
    {
        public abstract void UpdateObserver();
        
        protected virtual void LateUpdate()
        {
            if (Brain != null) UpdateObserver();
        }
    }
}