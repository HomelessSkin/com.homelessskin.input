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
        [Space]
        [SerializeField] Data[] Actions;

        public string GetGroup() => Group;
        public Data[] GetActions() => Actions;

        #region DATA
        [Serializable]
        public class Data
        {
            [HideInInspector] public string Name;
            public Key Key;
            public Perform.Data.Type Type;
            public string Title;
            public Command Command;
            [Space]
            public Action _Action;

            [Serializable]
            public class Action
            {
                public bool RemoveInput;
                [Space]
                public UnityEvent<OuterInput, Command> Event;
            }
        }
        #endregion

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
                    else if (action.Command)
                    {
                        action.Type = Perform.Data.Type.Outer;
                        action.Command.TryGetReward(out var reward);
                        action.Title = reward.title;
                    }

                    action.Name = $"On {action.Type} {action.Title}";
                }
        }
#endif
    }
}