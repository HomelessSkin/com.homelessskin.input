using System;

using Core;

using Unity.Collections;

using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "Chat Moderation", menuName = "Input/Processors/Chat Moderation")]
    public class ChatModeration : Processor
    {
        public override string JSONPath => "Chat Moderation/";
        public override Command Command => command;

        [Space]
        public ModerationCommand command;
    }

    [Serializable]
    public class ModerationCommand : Command
    {
        [Space]
        [Tooltip("Maximum 6-7 Words a Key")]
        [LogInfo] public Input.Key[] Keys;

        public override Key[] GetKeys(int index)
        {
            if (Keys == null || Keys.Length == 0)
                return null;

            var keys = new Key[Keys.Length];
            for (int k = 0; k < Keys.Length; k++)
            {
                var key = Keys[k];
                var cuts = new FixedList32Bytes<int>();
                for (int c = 0; c < key.Cuts.Length; c++)
                    cuts.Add(key.Cuts[c].ToLower().GetHashCode());

                keys[k] = new Key
                {
                    CompareType = Key.Type.ByAny,
                    IsPublic = IsPublic,
                    Index = index,

                    Cuts = cuts,
                };
            }

            return keys;
        }
        public override string[] GetPhrases() => null;

        protected override void Invoke(string data, ref Response response)
        {
            response = Response.StopInteractionNow;
        }
    }
}