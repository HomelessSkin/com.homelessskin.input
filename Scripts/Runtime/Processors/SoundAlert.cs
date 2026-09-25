using System;
using System.IO;

using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Input
{
    [CreateAssetMenu(fileName = "Sound Alert", menuName = "Input/Processors/Sound Alert")]
    public class SoundAlert : Processor
    {
        public override string JSONPath => "Sound Alerts/";

        public override Command Command => command;

        [Space]
        public SoundAlertCommand command;

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            command.Input.Title = "Sound Alert";
        }
#endif
    }

    [Serializable]
    public class SoundAlertCommand : Command
    {
        [Space]
        [LogInfo] public string Folder;

        [Space]
        [LogInfo] public Input.Key[] Keys;

        [Space]
        [LogInfo] public Clip[] Clips;

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
            var index = 0;
            if (Clips.Length > 1)
                index = Random.Range(1, Clips.Length);

            var clip = new Clip(Clips[index]);
            clip.Path = Path.Combine("file://", Application.persistentDataPath, Folder, clip.Path + ".mp3");

            var input = new OuterInput(Input);
            input.Message = JsonUtility.ToJson(clip);

            Sys.Add_M(input, World.DefaultGameObjectInjectionWorld.EntityManager);
        }
    }
}