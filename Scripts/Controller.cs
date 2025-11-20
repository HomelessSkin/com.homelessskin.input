using System;

using Core.Util;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] GroupTag Group;
        [SerializeField] Data[] Actions;

        public GroupTag GetGroup() => Group;
        public Data[] GetActions() => Actions;

        [Serializable]
        public class Data
        {
            public Key Key;
            public Input.Data.Type Type;
            [Space]
            public Action _Action;

            [Serializable]
            public class Action
            {
                public bool RemoveInput;
                [Space]
                public UnityEvent Event;
            }
        }

        public enum GroupTag : byte
        {
            Null = 0,
            UI = 1,
            Game = 2,

        }

#if UNITY_EDITOR
        void Reset()
        {
            Tool.CreateTag("InputController");
            gameObject.tag = "InputController";
        }
#endif
    }
}