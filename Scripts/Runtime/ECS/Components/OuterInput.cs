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

        public string Nick;
        public string NickColor;

        public string RewardID;
        public int Points;

        public bool IsSlashMe;
        public List<Badge> Badges;
        public List<Part> UserInput;

        public class Part
        {
            public Text Message;
            public Smile Emote;
            public Mention Reply;

            public class Text
            {
                public string Content;
            }

            public class Smile
            {
                public int Hash;
                public string URL;
            }

            public class Mention
            {
                public string Nick;
            }
        }

        public class Badge
        {
            public int Hash;
            public string SetID;
            public string ID;
            public string URL;
        }

        public class End : IComponentData
        {
            public LogLevel Result;
            public OuterInput Input;
        }
    }
}