using Core;

using UnityEngine;

namespace Input
{
    public abstract class PointerSettings : ScriptableSettings
    {
        public bool LogActivity = false;
        public bool LogActions = false;
        public bool LogStates = false;
        
        [Space]
        public float Freequency = 0.032f;
    }
}