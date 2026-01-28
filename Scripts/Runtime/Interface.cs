using System.Collections.Generic;
using System.Threading.Tasks;

namespace Input
{
    public interface IInteractable
    {
        Task<string> GetRewards();
        Task<bool> AddReward(Reward name);
        Task RemoveRewards(List<Reward> rewards);
        Task UpdateRewardRedemption(string status, OuterInput input);
    }
}