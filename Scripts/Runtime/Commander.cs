using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Core;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

using UnityEngine;

namespace Input
{
    public class Commander : ResourceLoader
    {
        [Space]
        [SerializeField] string JSONFolder = "/Input";

        [Space]
        [SerializeField] Processor[] Processors;

        int Index = 0;

        NativeList<Command.Key> Keys;
        List<Command> Commands = new List<Command>();

        [Space]
        [SerializeField] string[] Trimming;

        void Start()
        {
            ReloadCommands();
        }
        void OnDestroy()
        {
            if (Keys.IsCreated)
                Keys.Dispose();
        }

        public async void ReloadCommands()
        {
            Index = 0;

            if (Keys.IsCreated)
                Keys.Clear();
            else
                Keys = new NativeList<Command.Key>(Allocator.Persistent);

            Commands.Clear();

            var path = Path.Combine(Application.persistentDataPath, JSONFolder);
            var folders = new List<string>();
            for (int p = 0; p < Processors.Length; p++)
            {
                var processor = Processors[p];
                await AddCommand(processor.Command);

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
                        await AddCommand(await LoadCommand(files[f], type));
                }
            }

            async Task AddCommand(Command command)
            {
                await Task.Delay(32);

                Keys.AddRange(command.GetKeys(Index++));
                Commands.Add(command);

                Log.Info(this, $"Added Command of Type: {command.GetType().FullName}");
                Log.Object(this, command);
            }
            async Task<Command> LoadCommand(string file, Type type)
            {

                var text = await File.ReadAllTextAsync(file);
                if (!string.IsNullOrEmpty(text))
                {
                    var obj = JsonUtility.FromJson(text, type);

                    return obj as Command;
                }

                return null;
            }
        }
        public void Process(string data, bool isInternal = true)
        {
            data = data.ToLower();
            for (int t = 0; t < Trimming.Length; t++)
                data = data.Replace(Trimming[t], "");

            var message = new NativeList<int>(Allocator.TempJob);
            var arr = data.Split();
            for (int a = 0; a < arr.Length; a++)
            {
                var sub = arr[a];
                if (sub.Contains("<") || sub.Contains(">") || string.IsNullOrEmpty(sub))
                    continue;

                message.Add(sub.GetHashCode());
            }

            if (message.Length > 0)
            {
                Log.Info(this, $"Processing Voice Data:\n{data}");

                var stream = new NativeStream(Keys.Length, Allocator.TempJob);

                new KeyJob
                {
                    IsInternal = isInternal,

                    Keys = Keys,
                    Message = message,

                    Writer = stream.AsWriter()
                }
                .Schedule(Keys.Length, Keys.Length / JobsUtility.JobWorkerCount)
                .Complete();

                var stop = false;
                var reader = stream.AsReader();
                for (int f = 0; f < reader.ForEachCount; f++)
                {
                    reader.BeginForEachIndex(f);
                    while (reader.RemainingItemCount > 0)
                    {
                        reader.Read<bool>();

                        stop = Commands[Keys[f].Index].Call(data);
                        if (stop)
                            break;
                    }
                    reader.EndForEachIndex();

                    if (stop)
                        break;
                }

                stream.Dispose();
            }
            else
                Log.Info(this, $"Data Message is empty!");

            message.Dispose();
        }

        [BurstCompile]
        struct KeyJob : IJobParallelFor
        {
            [ReadOnly] public bool IsInternal;

            [ReadOnly] public NativeList<Command.Key> Keys;
            [ReadOnly, NativeDisableParallelForRestriction] public NativeList<int> Message;

            public NativeStream.Writer Writer;

            public void Execute(int index)
            {
                var key = Keys[index];
                if (!IsInternal && !key.IsPublic)
                    return;

                var isFits = true;
                switch (key.CompareType)
                {
                    case Command.Key.Type.ByFirst:
                    isFits = Message[0] == key.Cuts[0];
                    break;

                    case Command.Key.Type.ByAll:
                    for (int c = 0; c < key.Cuts.Length; c++)
                    {
                        isFits &= Contains(key.Cuts[c]);

                        if (!isFits)
                            break;
                    }
                    break;
                }

                if (isFits)
                {
                    Writer.BeginForEachIndex(index);
                    Writer.Write(true);
                    Writer.EndForEachIndex();
                }
            }

            bool Contains(int id)
            {
                for (int m = 0; m < Message.Length; m++)
                    if (Message[m] == id)
                        return true;

                return false;
            }
        }

#if UNITY_EDITOR
        protected override void LoadResources() => Load<Processor>(ref Processors);
#endif
    }
}