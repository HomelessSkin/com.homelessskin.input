using System;
using System.Collections.Generic;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract partial class HandleSystem : ReloadManagedSingletoneSystem<Perform>
    {
        protected static string RulesPath;

        protected int ActionDataLength;
        protected string Group;
        protected Controller Controller;

        protected Dictionary<int, InnerAction[]> InnerActions = new Dictionary<int, InnerAction[]>();
        protected Dictionary<int, Controller.Data[]> OuterActions = new Dictionary<int, Controller.Data[]>();

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<Perform>();

            ActionDataLength = Enum.GetValues(typeof(Perform.Data.Type)).Length;
        }
        protected override void OnUpdate()
        {
            ProcessOuterActions();

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
                        AddOuterAction(events[e]);
                }
            }
        }
        protected override void Proceed()
        {
            var toRemove = new List<int>();

            var innerActions = new List<UnityAction>();
            var outerActions = new List<(Controller.Data, Perform.Data)>();

            for (int v = 0; v < Value._Data.Count; v++)
            {
                var input = Value._Data[v];

                if (InnerActions.TryGetValue(input.Key, out var inner))
                {
                    var action = inner[(int)input._Type];
                    if (action != null)
                    {
                        innerActions.Add(action.Action);

                        if (!action.RemoveInput)
                            toRemove.Add(v);
                    }
                }

                if (OuterActions.TryGetValue(input.Key, out var outer))
                {
                    var action = outer[(int)input._Type];
                    if (action != null)
                    {
                        outerActions.Add((action, input));

                        if (action._Action.RemoveInput)
                            toRemove.Add(v);
                    }
                }
            }

            for (int t = toRemove.Count - 1; t >= 0; t--)
                Value._Data.RemoveAt(toRemove[t]);

            for (int i = 0; i < innerActions.Count; i++)
                innerActions[i].Invoke();

            for (int o = 0; o < outerActions.Count; o++)
            {
                var action = outerActions[o];

                if (action.Item1.Command)
                    action.Item1.Command.TryGetReward(out var r);

                action.Item1._Action.Event?.Invoke(action.Item2.Input, action.Item1.Command);
            }
        }

        protected virtual void AddInnerAction(Key button, Perform.Data.Type type, bool removeInput, UnityAction action)
        {
            var key = button.ToString().GetHashCode();
            if (!InnerActions.TryGetValue(key, out var actions))
                actions = new InnerAction[ActionDataLength];

            actions[(int)type] = new InnerAction { Key = button, Type = type, RemoveInput = removeInput, Action = action };
            InnerActions[key] = actions;
        }
        protected virtual void ProcessOuterActions()
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
                    AddOuterAction(request.Data);

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
                    RemoveOuterAction(request.Key);

                    toDestroy.Add(entity);
                }
            }

            for (int e = 0; e < toDestroy.Count; e++)
                EntityManager.DestroyEntity(toDestroy[e]);
        }
        protected virtual void AddOuterAction(Controller.Data data)
        {
            if (string.IsNullOrEmpty(data.Title))
                return;

            var key = data.Title.GetHashCode();
            if (!OuterActions.TryGetValue(key, out var actions))
                actions = new Controller.Data[ActionDataLength];

            var index = (int)data.Type;
            if (actions[index] != null)
                actions[index]._Action.Event?.RemoveAllListeners();

            actions[index] = data;
            OuterActions[key] = actions;
        }
        protected virtual void RemoveOuterAction(int key)
        {
            if (OuterActions.TryGetValue(key, out var data))
            {
                for (int d = 0; d < ActionDataLength; d++)
                    if (data[d] != null)
                        data[d]._Action.Event?.RemoveAllListeners();

                OuterActions.Remove(key);
            }
        }

        #region INNER ACTION
        protected class InnerAction
        {
            public Key Key;
            public Perform.Data.Type Type;
            [Space]
            public bool RemoveInput;
            public UnityAction Action;
        }
        #endregion
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