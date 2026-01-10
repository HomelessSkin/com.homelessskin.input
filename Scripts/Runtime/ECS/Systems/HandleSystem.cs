using System;
using System.Collections.Generic;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Input
{
    public abstract partial class HandleSystem : ReloadManagedSingletoneSystem<Input>
    {
        protected static string RulesPath;

        protected int ActionDataLength;
        protected string Group;
        protected Controller Controller;

        protected Dictionary<int, Controller.Data[]> Actions = new Dictionary<int, Controller.Data[]>();

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<Input>();

            ActionDataLength = Enum.GetValues(typeof(Input.Data.Type)).Length;
        }
        protected override void OnUpdate()
        {
            ProcessRequestedActions();

            base.OnUpdate();
        }
        protected override void GetRef()
        {
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
                        AddAction(events[e]);
                }
            }
        }
        protected override void Proceed()
        {
            var toRemove = new List<int>();
            var toInvoke = new List<(Controller.Data, Input.Data)>();

            for (int k = 0; k < Value._Data.Count; k++)
            {
                var input = Value._Data[k];
                if (!Actions.TryGetValue(input.Key, out var actions))
                    continue;

                if (actions[(int)input._Type] != null)
                {
                    toInvoke.Add((actions[(int)input._Type], input));

                    if (actions[(int)input._Type]._Action.RemoveInput)
                        toRemove.Add(k);
                }
            }

            for (int t = toRemove.Count - 1; t >= 0; t--)
                Value._Data.RemoveAt(toRemove[t]);

            for (int t = 0; t < toInvoke.Count; t++)
            {
                var tI = toInvoke[t];

                if (tI.Item1.Command)
                    tI.Item1.Command.TryGetReward(out var r);

                tI.Item1._Action.Event?.Invoke(tI.Item2.Event, tI.Item1.Command);
            }
        }

        protected virtual void ProcessRequestedActions()
        {
            var added = EntityManager.CreateEntityQuery(typeof(AddActionRequest));
            var removed = EntityManager.CreateEntityQuery(typeof(RemoveActionRequest));
            var toDestroy = new List<Entity>();

            var addEs = added.ToEntityArray(Allocator.Temp);
            for (int e = 0; e < addEs.Length; e++)
            {
                var entity = addEs[e];
                var request = EntityManager.GetComponentObject<AddActionRequest>(entity);
                if (Group == request.Group)
                {
                    AddAction(request.Data);

                    toDestroy.Add(entity);
                }
            }

            var remEs = removed.ToEntityArray(Allocator.Temp);
            for (int e = 0; e < remEs.Length; e++)
            {
                var entity = remEs[e];
                var request = EntityManager.GetComponentObject<RemoveActionRequest>(entity);
                if (Group == request.Group)
                {
                    RemoveAction(request.Key);

                    toDestroy.Add(entity);
                }
            }

            for (int e = 0; e < toDestroy.Count; e++)
                EntityManager.DestroyEntity(toDestroy[e]);
        }
        protected virtual void AddAction(Controller.Data data)
        {
            var key = 0;
            if (data.Command)
            {
                data.Command.TryGetReward(out var reward);
                key = reward.title.GetHashCode();
            }
            else
                key = data.Key.ToString().GetHashCode();


            if (!Actions.TryGetValue(key, out var actions))
                actions = new Controller.Data[ActionDataLength];

            var index = (int)data.Type;
            if (actions[index] != null)
                actions[index]._Action.Event?.RemoveAllListeners();

            actions[index] = data;
            Actions[key] = actions;
        }
        protected virtual void RemoveAction(int key)
        {
            if (Actions.TryGetValue(key, out var data))
            {
                for (int d = 0; d < ActionDataLength; d++)
                    if (data[d] != null)
                        data[d]._Action.Event?.RemoveAllListeners();

                Actions.Remove(key);
            }
        }
    }

    #region REQUEST
    public class AddActionRequest : IComponentData
    {
        public string Group;
        public Controller.Data Data;
    }
    public class RemoveActionRequest : IComponentData
    {
        public string Group;
        public int Key;
    }
    #endregion
}