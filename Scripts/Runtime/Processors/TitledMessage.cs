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
                CommandIndex = index,
                Index = 0,
                Priority = Priority,

                Cuts = new FixedList32Bytes<int>
                {
                    Title.ToLower().GetHashCode()
                }
            }
        };
        public override string[] GetPhrases() => new string[] { Title };

        protected override void Invoke(ref string data, ref Response response, Key key)
        {
            var message = data.Trim().ToLower();

            var input = new OuterInput(Input);
            input.Message = data.Replace(Title.ToLower(), "").Trim();

            Sys.Add_M(input, World.DefaultGameObjectInjectionWorld.EntityManager);

            response = Response.Nominal;
        }
    }
}