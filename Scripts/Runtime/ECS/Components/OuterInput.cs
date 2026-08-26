using System;
using System.Collections.Generic;

using Unity.Entities;

using static Core.Log;

namespace Input
{
    [Serializable]
    public class OuterInput : IComponentData, ILogTarget
    {
        [LogInfo] public string Title;
        [LogInfo] public string ID;
        [LogInfo] public string Source = "this";
        [LogInfo] public string Agent = "unknown";
        [LogInfo] public int Cost = 0;
        [LogInfo] public string Message;

        [NonSerialized] public string UserID;
        [NonSerialized] public string RewardID;

        [NonSerialized] public List<int> Icons;
        [NonSerialized] public List<int> Badges;

        public OuterInput() { }
        public OuterInput(string title)
        {
            Title = title;
        }
        public OuterInput(string title, string message)
        {
            Title = title;
            Message = message;
        }
        public OuterInput(OuterInput input)
        {
            Title = input.Title;
            ID = input.ID;
            Source = input.Source;
            Agent = input.Agent;
            Cost = input.Cost;
            Message = input.Message;

            UserID = input.UserID;
            RewardID = input.RewardID;

            if (input.Icons != null && input.Icons.Count > 0)
            {
                Icons = new List<int>();
                Icons.AddRange(input.Icons);
            }

            if (input.Badges != null && input.Badges.Count > 0)
            {
                Badges = new List<int>();
                Badges.AddRange(input.Badges);
            }
        }
    }
}