using System;

using Core;

using UnityEngine;

namespace Input
{
    public abstract class Processor : KeyScriptable, ILogTarget
    {
        public abstract string JSONPath { get; }
        [LogInfo] public abstract Command Command { get; }

#if UNITY_EDITOR
        [Space]
        [TextArea(20, 50)] public string JSON;

        protected override void Reset()
        {
            base.Reset();

            Command.Input = new OuterInput();
            Command.Input.Source = "Message";
            Command.Input.Agent = "Chatter";
        }

        void OnValidate()
        {
            JSON = JsonUtility.ToJson(Command, true);
        }
#endif
    }

    [Serializable]
    public abstract class Command : ILogTarget
    {
        public bool IsPublic;

        [Space]
        [LogInfo] public OuterInput Input;

        public bool Call(string data, bool isInternal)
        {
            if (IsPublic || isInternal)
                return Invoke(data);

            return false;
        }

        protected abstract bool Invoke(string data);
    }
}