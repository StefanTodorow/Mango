using Mango.Service.EmailAPI.Models;
using Mango.Services.EmailAPI.Data;
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

            await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
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
