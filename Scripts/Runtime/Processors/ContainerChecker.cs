using System;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "Container Checker", menuName = "Input/Processors/Container Checker")]
    public class ContainerChecker : Processor
    {
        public override string JSONPath => "Container Checkers/";
        public override Command Command => command;

        [Space]
        public ContainerCheckerCommand command;
    }

    [Serializable]
    public class ContainerCheckerCommand : Command
    {
        [Space]
        [LogInfo] public Input.Key[] Keys;

        public override Key[] GetKeys(int index)
        {
            var keys = new Key[Keys.Length];
            for (int k = 0; k < Keys.Length; k++)
            {
                var key = Keys[k];
                var cuts = new FixedList32Bytes<int>();
                for (int c = 0; c < key.Cuts.Length; c++)
                    cuts.Add(key.Cuts[c].GetHashCode());

                keys[k] = new Key
                {
                    CompareType = Key.Type.ByAll,
                    IsPublic = IsPublic,
                    Index = index,

                    Cuts = cuts,
                };
            }

            return keys;
        }

        protected override void Invoke(string data)
        {
            Sys.Add_M(Input, World.DefaultGameObjectInjectionWorld.EntityManager);
        }
    }
}