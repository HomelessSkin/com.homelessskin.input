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
            public Command Command;
            public Key Key;
            public Perform.Data.Type Type;
            [Space]
            public Action _Action;

            [Serializable]
            public class Action
            {
                public bool RemoveInput;
                [Space]
                public UnityEvent<IInteractable.Event, Command> Event;
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
                    var keyStr = "";
                    if (action.Command)
                    {
                        action.Command.TryGetReward(out var reward);

                        keyStr = reward.title;
                        action.Type = Perform.Data.Type.Command;
                    }
                    else
                        keyStr = action.Key.ToString();

                    action.Name = $"On {keyStr} {action.Type}";
                }
        }
#endif
    }
}