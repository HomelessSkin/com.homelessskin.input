using System.Collections.Generic;

using Core;

using Unity.Entities;

namespace Input
{
    public class OuterInput : IComponentData
    {
        public string Platform;

        public string Title;
        public string ID;

        public string UserID;
        public string Nick;
        public string NickColor;

        public string RewardID;
        public int Points;

        public bool IsSlashMe;
        public List<Icon> Badges;
        public List<Part> UserInput;

        public class Part
        {
            public Text Message;
            public Icon Emote;
            public Mention Reply;

            public class Text
            {
                public string Content;
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

        public class End : IComponentData
        {
            public LogLevel Result;
            public OuterInput Input;
        }
    }
}