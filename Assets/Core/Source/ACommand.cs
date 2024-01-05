using UnityEngine;

namespace Core.Source
{
    public abstract class ACommand : ScriptableObject
    {
        public string CommandName;

        public bool IsAvailable()
        {
            return true && !IsLocked();
        }

        public bool IsLocked()
        {
            return false;
        }

        public virtual void Select()
        {

        }
    }
}