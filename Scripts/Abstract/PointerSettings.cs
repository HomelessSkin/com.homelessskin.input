using UnityEngine;

namespace Input
{
    public abstract class PointerSettings : ScriptableObject
    {
        public bool LogActions = false;
        public float Freequency = 0.032f;
    }
}
