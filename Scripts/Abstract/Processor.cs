using System;

using Core;

using Unity.Collections;

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
        public bool IsFinal;

        [Space]
        [LogInfo] public OuterInput Input;

        public abstract Key[] GetKeys(int index);

        public bool Call(string data)
        {
            Invoke(data);

            return IsFinal;
        }

        protected abstract void Invoke(string data);

        public struct Key
        {
            public Type CompareType;
            public bool IsPublic;
            public int Index;

            public FixedList32Bytes<int> Cuts;

            public enum Type : byte
            {
                ByFirst = 0,
                ByAll = 1,
            }
        }
    }
    [Serializable]
    public class Key : ILogTarget
    {
        [LogInfo] public string[] Cuts;
    }
}