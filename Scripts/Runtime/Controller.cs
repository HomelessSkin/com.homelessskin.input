using System;

using Core;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] string Group;
        [SerializeField] PointerSettings PointerSettings;

        [Space]
        [SerializeField] Data[] Actions;

        protected ControllerState State;

        public ControllerState GetState() => State;

        protected virtual void SetState(ControllerState state)
        {
            State = state;
        }

        public string GetGroup() => Group;
        public PointerSettings GetPointerSettings() => PointerSettings;
        public Data[] GetActions() => Actions;

        #region DATA
        [Serializable]
        public class Data
        {
            [HideInInspector] public string Name;
            public Key Key;
            public Perform.Data.Type Type;
            public string Title;
            [Space]
            public Action _Action;

            [Serializable]
            public class Action
            {
                public bool RemoveInput;
                [Space]
                public UnityEvent<OuterInput> Event;
            }
        }
        #endregion

        public enum ControllerState : byte
        {
            Main = 0,
            Started = 1,
            Playing = 2,
            Stoped = 3,
            Closed = 4,
            Paused = 5,

        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Tool.CreateTag("InputController");
            gameObject.tag = "InputController";
        }
        protected virtual void OnValidate()
        {
            if (Actions != null)
                for (int a = 0; a < Actions.Length; a++)
                {
                    var action = Actions[a];

                    if (action.Key != Key.None)
                        action.Title = action.Key.ToString();
                    else if (!string.IsNullOrEmpty(action.Title))
                        action.Type = Perform.Data.Type.Outer;

                    action.Name = $"On {action.Type} {action.Title}";
                }
        }
#endif
    }
}