using System;
using System.Collections.Generic;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine.InputSystem;

namespace Input
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial class CollectSystem : ReloadManagedSingletoneSystem<Perform>
    {
        protected override void GetRef() { }
        protected override void Proceed()
        {
            if (Value == null)
                return;

            var unended = new List<OuterInput.End>();
            for (int d = 0; d < Value._Data.Count; d++)
                if (Value._Data[d]._Type == Perform.Data.Type.Outer)
                    unended.Add(new OuterInput.End { Result = LogLevel.Warning, Input = Value._Data[d].Input });

            Value._Data.Clear();

            var keyboard = Keyboard.current;
            if (keyboard != null)
                for (int k = 0; k < keyboard.allKeys.Count; k++)
                {
                    var key = keyboard.allKeys[k];
                    if (key == null)
                        continue;

                    if (key.wasPressedThisFrame)
                        Value._Data.Add(new Perform.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Perform.Data.Type.Down
                        });
                    else if (key.wasReleasedThisFrame)
                        Value._Data.Add(new Perform.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Perform.Data.Type.Up
                        });
                    else if (key.isPressed)
                        Value._Data.Add(new Perform.Data
                        {
                            Key = key.keyCode.ToString().GetHashCode(),
                            _Type = Perform.Data.Type.Hold
                        });
                }

            var query = EntityManager.CreateEntityQuery(typeof(OuterInput));
            if (!query.IsEmpty)
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                for (int e = 0; e < entities.Length; e++)
                {
                    var input = EntityManager.GetComponentObject<OuterInput>(entities[e]);

                    Value._Data.Add(new Perform.Data
                    {
                        Key = input.Title.GetHashCode(),
                        _Type = Perform.Data.Type.Outer,
                        Input = input,
                    });
                }

                EntityManager.DestroyEntity(query);
            }

            for (int u = 0; u < unended.Count; u++)
                Sys.Add_M(unended[u], EntityManager);
        }
    }

    #region INPUT
    public class Perform : IComponentData
    {
        public List<Data> _Data = new List<Data>();

        [Serializable]
        public class Data
        {
            public int Key;
            public Type _Type;
            public OuterInput Input;

            public enum Type : byte
            {
                Null = 0,
                Down = 1,
                Hold = 2,
                Up = 3,
                Outer = 4,
            }
        }
    }
    #endregion
}