using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Core;

using UnityEngine;

namespace Input
{
    public class Commander : ResourceLoader
    {
        [Space]
        [SerializeField] string JSONFolder = "/Input";

        [Space]
        [SerializeField] Processor[] Processors;

        List<Command> Commands = new List<Command>();

        [Space]
        [SerializeField] string[] Trimming;

        void Start()
        {
            ReloadCommands();
        }

        public async void ReloadCommands()
        {
            Commands.Clear();

            var path = Path.Combine(Application.persistentDataPath, JSONFolder);
            var folders = new List<string>();
            for (int p = 0; p < Processors.Length; p++)
            {
                var processor = Processors[p];
                Commands.Add(processor.Command);

                if (!folders.Contains(processor.JSONPath))
                {
                    folders.Add(processor.JSONPath);

                    var folder = Path.Combine(path, processor.JSONPath);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);

                        continue;
                    }

                    var files = Directory.GetFiles(folder, "*.json");
                    if (files == null || files.Length == 0)
                        continue;

                    var type = processor.Command.GetType();
                    for (int f = 0; f < files.Length; f++)
                    {
                        await Task.Delay(1000);
                        await AddCommand(files[f], type);
                    }
                }
            }

            async Task AddCommand(string file, Type type)
            {
                var text = await File.ReadAllTextAsync(file);
                if (!string.IsNullOrEmpty(text))
                {
                    var obj = JsonUtility.FromJson(text, type);
                    Commands.Add(obj as Command);

                    Log.Info(this, $"Added Command of Type: {type.FullName}");
                    Log.Object(this, obj as ILogTarget);
                }
            }
        }
        public void Process(string data, bool isInternal = true)
        {
            data = data.ToLower();
            for (int t = 0; t < Trimming.Length; t++)
                data = data.Replace(Trimming[t], "");

            Log.Info(this, $"Processing Voice Data:\n{data}");

            for (int c = 0; c < Commands.Count; c++)
                if (Commands[c].Call(data, isInternal))
                    return;
        }

#if UNITY_EDITOR
        protected override void LoadResources() => Load<Processor>(ref Processors);
#endif
    }
}