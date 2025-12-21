using System;
using System.Collections.Generic;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine.InputSystem;

namespace Input
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SpawnSystemGroup))]
    public partial class InputSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial class CollectSystem : ReloadManagedSingletoneSystem<Input>
    {
        protected override void GetRef() { }
        protected override void Proceed()
        {
            if (Value == null)
                return;

            var unended = new List<EndEvent>();
            for (int d = 0; d < Value._Data.Count; d++)
                if (Value._Data[d]._Type == Input.Data.Type.Command)
                    unended.Add(new EndEvent { Result = LogLevel.Warning, Event = Value._Data[d].Event });

            Value._Data.Clear();

            var keyboard = Keyboard.current;
            if (keyboard != null)
                for (int k = 0; k < keyboard.allKeys.Count; k++)
                {
                    var key = keyboard.allKeys[k];
                    if (key == null)
                        continue;

                    if (key.wasPressedThisFrame)
                        Value._Data.Add(new Input.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Input.Data.Type.Down
                        });
                    else if (key.wasReleasedThisFrame)
                        Value._Data.Add(new Input.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Input.Data.Type.Up
                        });
                    else if (key.isPressed)
                        Value._Data.Add(new Input.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Input.Data.Type.Hold
                        });
                }

            var query = EntityManager.CreateEntityQuery(typeof(IInteractable.Event));
            if (!query.IsEmpty)
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                for (int e = 0; e < entities.Length; e++)
                {
                    var @event = EntityManager.GetComponentObject<IInteractable.Event>(entities[e]);

                    Value._Data.Add(new Input.Data
                    {
                        Key = @event.Title.GetHashCode(),
                        _Type = Input.Data.Type.Command,
                        Event = @event,
                    });
                }

                EntityManager.DestroyEntity(query);
            }

            for (int u = 0; u < unended.Count; u++)
                Sys.Add_M(unended[u], EntityManager);
        }
    }

    public class Input : IComponentData
    {
        public List<Data> _Data = new List<Data>();

        [Serializable]
        public class Data
        {
            public int Key;
            public Type _Type;
            public IInteractable.Event Event;

            public enum Type : byte
            {
                Null = 0,
                Down = 1,
                Hold = 2,
                Up = 3,
                Command = 4,
            }
        }
    }

    public class EndEvent : IComponentData
    {
        public LogLevel Result;
        public IInteractable.Event Event;
    }
}