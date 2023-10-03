using Mango.Services.RewardAPI.Messaging;

namespace Mango.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateReward(RewardMessage rewardMessage);
    }
}
