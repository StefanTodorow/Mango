using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Messaging;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbContextOptions;

        public RewardService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task UpdateReward(RewardMessage rewardMessage)
        {
            try
            {
                Reward reward = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardActivity = rewardMessage.RewardActivity,
                    UserId = rewardMessage.UserId,
                    RewardDate = DateTime.Now
                };

                await using var db = new AppDbContext(_dbContextOptions);
                await db.Rewards.AddAsync(reward);
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
