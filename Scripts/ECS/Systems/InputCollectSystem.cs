using System;

using Core.Util;

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
    public partial class InputCollectSystem : ReloadSingletoneSystem<Input>
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
                        Value.ValueRW._Data.Add(new Input.Data { });
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

        }
    }
}