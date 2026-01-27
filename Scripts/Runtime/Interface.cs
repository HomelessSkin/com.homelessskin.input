using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Core;

using Unity.Entities;

namespace Input
{
    public interface IInteractable
    {
        void EnqueueEvent(Event @event) => _RewardEvents.Enqueue(@event);
        Task<string> GetRewards();
        Task<bool> AddReward(Reward name);
        Task RemoveRewards(List<Reward> rewards);
        Task UpdateRewardRedemption(string status, IInteractable.Event @event);

        public bool GetEvent(out Event @event) => _RewardEvents.TryDequeue(out @event);
        public async Task CancelRewards()
        {
            while (_RewardEvents.TryDequeue(out var @event))
                await UpdateRewardRedemption("CANCELED", @event);
        }

        public Queue<Event> _RewardEvents { get; set; }

        [Serializable]
        public class Event : IComponentData
        {
            public string Platform;
            public string Title;
            public string Nick;
            public string NickColor;
            public string RewardID;
            public string UserInput;
            public string ID;
            public int Points;
        }

        public class EndEvent : IComponentData
        {
            public LogLevel Result;
            public Event Event;
        }
    }
}