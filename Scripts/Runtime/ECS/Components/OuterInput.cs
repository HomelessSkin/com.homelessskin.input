using System.Collections.Generic;

using Unity.Entities;

using static Core.Log;

namespace Input
{
    public class OuterInput : IComponentData, ILogTarget
    {
        [LogInfo] public string Platform = "overlay";
        [LogInfo] public string RewardID;
        [LogInfo] public string Title;
        [LogInfo] public string ID;

        [LogInfo] public string UserID;
        [LogInfo] public string Nick = "unknown";
        [LogInfo] public int Points = 0;

        [LogInfo] public List<Part> UserInput;

        public int Index;
        public bool IsSlashMe;
        public string NickColor;
        public List<Icon> Badges;

        public class Part : ILogTarget
        {
            [LogInfo] public Text Message;

            public Icon Emote;
            public Mention Reply;

            public class Text : ILogTarget
            {
                [LogInfo] public string Content;
            }

            public class Mention
            {
                public string Nick;
            }
        }

        public class Icon
        {
            public int Hash;
            public int Index;
        }

        public OuterInput() { }
        public OuterInput(string title)
        {
            Title = title;
        }
    }
}