using Mango.Service.EmailAPI.Models;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Service.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbContextOptions;

        public EmailService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }
        public async Task EmailCartAndLogAsync(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total: " + cartDTO.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Quantity);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmailAsync(message.ToString(), cartDTO.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardMessage rewardsDto)
        {
            string message = "New Order Placed. <br/> Order ID: " + rewardsDto.OrderId;
            await LogAndEmailAsync(message, "test@test.com");
        }

        public async Task RegisterUserEmailAndLogAsync(string email)
        {
            string message = "User registration was successful. <br/> Email : " + email;
            await LogAndEmailAsync(message, email);
        }

        private async Task<bool> LogAndEmailAsync(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var db = new AppDbContext(_dbContextOptions);
                await db.EmailLoggers.AddAsync(emailLog);
                await db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
