using System.Collections.Generic;

using Unity.Entities;

using static Core.Log;

namespace Input
{
    public class OuterInput : IComponentData, ILogTarget
    {
        [LogInfo] public string Title;
        [LogInfo] public string ID;
        [LogInfo] public string Source = "this";
        [LogInfo] public string Agent = "unknown";
        [LogInfo] public int Cost = 0;
        [LogInfo] public string Message;

        public string UserID;
        public string RewardID;

        public List<int> Icons;
        public List<int> Badges;

        public OuterInput() { }
        public OuterInput(string title)
        {
            Title = title;
        }
    }
}