using System.Collections.Generic;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine.InputSystem;

namespace Input
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial class CollectSystem : ManagedSingletonSystem<Perform>
    {
        Dictionary<int, string> OuterMap = new Dictionary<int, string>();

        protected override void GetRef() { }
        protected override void Proceed()
        {
            for (int d = 0; d < Value._Data.Count; d++)
                if (OuterMap.TryGetValue(Value._Data[d].Key, out var name))
                    Log.Warning(this, $"Outer Input '{name}' was not released previous Frame!");

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
                            Key = key.keyCode.ToString().ToLower().GetHashCode(),
                            _Type = Perform.Data.Type.Down
                        });
                    else if (key.wasReleasedThisFrame)
                        Value._Data.Add(new Perform.Data
                        {
                            Key = key.keyCode.ToString().ToLower().GetHashCode(),
                            _Type = Perform.Data.Type.Up
                        });
                    else if (key.isPressed)
                        Value._Data.Add(new Perform.Data
                        {
                            Key = key.keyCode.ToString().ToLower().GetHashCode(),
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
                    if (input.Title != "Message")
                        Log.Object(this, input);

                    var key = input.Title.ToLower().GetHashCode();
                    if (!OuterMap.TryGetValue(key, out var name))
                        OuterMap[key] = input.Title;

                    Value._Data.Add(new Perform.Data
                    {
                        Key = key,
                        _Type = Perform.Data.Type.Outer,
                        Input = input,
                    });
                }

                EntityManager.DestroyEntity(query);
            }
        }
    }

    #region PERFORM
    public class Perform : IComponentData
    {
        public List<Data> _Data = new List<Data>();

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