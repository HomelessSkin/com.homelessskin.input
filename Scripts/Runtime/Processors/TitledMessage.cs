using System;

using Core;

using Input;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

namespace Whisper
{
    [CreateAssetMenu(fileName = "Titled Message", menuName = "Input/Processors/Titled Message")]
    public class TitledMessage : Processor
    {
        public override string JSONPath => "Titled Messages/";
        public override Command Command => command;

        [Space]
        public TitledMessageCommand command;
    }

    [Serializable]
    public class TitledMessageCommand : Command
    {
        [Space]
        [LogInfo] public string Title;

        public override Key[] GetKeys(int index) => new Key[]
        {
            new Key
            {
                CompareType = Key.Type.ByFirst,
                IsPublic = IsPublic,
                Index = index,

                Cuts = new FixedList32Bytes<int>
                {
                    Title.ToLower().GetHashCode()
                }
            }
        };

        protected override void Invoke(string data)
        {
            var message = data.Trim().ToLower();

            var input = new OuterInput(Input);
            input.Message = data.Replace(Title.ToLower(), "").Trim();

            Sys.Add_M(input, World.DefaultGameObjectInjectionWorld.EntityManager);
        }
    }
}