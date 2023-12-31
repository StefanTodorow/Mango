﻿using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Service.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLogAsync(CartDTO cartDTO);
        Task RegisterUserEmailAndLogAsync(string email);
        Task LogOrderPlaced(RewardMessage rewardsDto);
    }
}
