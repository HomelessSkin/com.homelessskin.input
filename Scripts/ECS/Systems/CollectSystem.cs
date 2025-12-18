using System;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine.InputSystem;

namespace Input
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSystemGroup : ComponentSystemGroup
    {

    }

    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial class CollectSystem : ReloadSingletoneSystem<Input>
    {
        protected override void Proceed()
        {
            Value.ValueRW._Data.Clear();

            var keyboard = Keyboard.current;
            if (keyboard != null)
                for (int k = 0; k < keyboard.allKeys.Count; k++)
                {
                    var key = keyboard.allKeys[k];
                    if (key == null)
                        continue;

                    if (key.wasPressedThisFrame)
                        Value.ValueRW._Data.Add(new Input.Data
                        {
                            Key = key.keyCode,
                            _Type = Input.Data.Type.Down
                        });
                    else if (key.wasReleasedThisFrame)
                        Value.ValueRW._Data.Add(new Input.Data
                        {
                            Key = key.keyCode,
                            _Type = Input.Data.Type.Up
                        });
                    else if (key.isPressed)
                        Value.ValueRW._Data.Add(new Input.Data
                        {
                            Key = key.keyCode,
                            _Type = Input.Data.Type.Hold
                        });
                }
        }
    }

    public struct Input : IDefaultable<Input>
    {
        public bool Initialized { get; set; }

        public NativeList<Data> _Data;

        public Input CreateDefault() => new Input
        {
            Initialized = true,

            _Data = new NativeList<Data>(Allocator.Persistent)
        };

        [Serializable]
        public struct Data
        {
            public Key Key;
            public Type _Type;

            public enum Type : byte
            {
                Null = 0,
                Down = 1,
                Hold = 2,
                Up = 3
            }
        }
    }
}