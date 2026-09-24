using System;
using System.IO;

using Core;

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
        [LogInfo] public string[] Keys;

        [Space]
        [LogInfo] public Clip[] Clips;

        protected override bool Invoke(string data)
        {
            var contains = true;
            var arr = data.Split();
            for (int k = 0; k < Keys.Length; k++)
            {
                contains &= Contains(Keys[k].ToLower());
                if (!contains)
                    break;
            }

            if (contains)
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

            return contains;

            bool Contains(string key)
            {
                for (int a = 0; a < arr.Length; a++)
                    if (arr[a].ToLower().Equals(key))
                        return true;

                return false;
            }
        }
    }
}