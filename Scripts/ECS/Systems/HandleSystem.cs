using System;
using System.Collections.Generic;

using Core;

using Unity.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract partial class HandleSystem : ReloadSingletoneSystem<Input>
    {
        protected static string RulesPath;

        protected Controller.GroupTag Group;
        protected Controller Controller;

        protected Dictionary<Key, Controller.Data.Action[]> Actions = new Dictionary<Key, Controller.Data.Action[]>();

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<Input>();
        }
        protected override void GetRef()
        {
            base.GetRef();

            if (!Controller)
            {
                var controllers = GameObject.FindGameObjectsWithTag("InputController");
                for (int c = 0; c < controllers.Length; c++)
                {
                    var controller = controllers[c].GetComponent<Controller>();
                    if (controller.GetGroup() == Group)
                    {
                        Controller = controller;

                        break;
                    }
                }

                if (Controller)
                {
                    var events = Controller.GetActions();
                    for (int e = 0; e < events.Length; e++)
                    {
                        var data = events[e];
                        if (!Actions.TryGetValue(data.Key, out var actions))
                            actions = new Controller.Data.Action[Enum.GetValues(typeof(Input.Data.Type)).Length];

                        actions[(int)data.Type] = data._Action;
                        Actions[data.Key] = actions;
                    }
                }
            }
        }
        protected override void Proceed()
        {
            var toRemove = new NativeList<int>(Allocator.Temp);
            for (int k = 0; k < Value.ValueRO._Data.Length; k++)
            {
                var butt = Value.ValueRO._Data[k];

                if (!Actions.TryGetValue(butt.Key, out var actions))
                    continue;

                if (actions[(int)butt._Type] != null)
                {
                    actions[(int)butt._Type].Event?.Invoke();

                    if (actions[(int)butt._Type].RemoveInput)
                        toRemove.Add(k);
                }
            }

            for (int t = toRemove.Length - 1; t >= 0; t--)
                Value.ValueRW._Data.RemoveAt(toRemove[t]);
        }
    }
}