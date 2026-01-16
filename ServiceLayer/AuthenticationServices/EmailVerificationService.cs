using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.AuthenticationDTOS;
using SystemModel.Entities;

namespace ServiceLayer.AuthenticationServices
{
    public class EmailVerificationService
    {
        private readonly DelivryDB _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public EmailVerificationService(
            DelivryDB context,
            EmailService emailService,
            IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task SendVerificationEmailAsync(User user)
        {
            var token = GenerateToken();

            user.EmailVerificationToken = token;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            user.IsEmailVerified = false;

            await _context.SaveChangesAsync();

            var link =
                $"{_config["Email:ClientUrl"]}/verify-email?token={token}";

            await _emailService.SendAsync(
                user.Email,
                "Verify your email",
                $@"
            <h3>Welcome to Delivery System</h3>
            <p>Click the link below to verify your email:</p>
            <a href='{link}'>Verify Email</a>
            <p>This link expires in 24 hours.</p>"
            );
        }

        public async Task VerifyEmailAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.EmailVerificationToken == token &&
                u.EmailVerificationTokenExpiry > DateTime.UtcNow);

            if (user == null)
                throw new Exception("Invalid or expired token");

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            await _context.SaveChangesAsync();
        }

        public async Task ResendVerificationAsync(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                throw new Exception("User not found");

            if (user.IsEmailVerified)
                throw new Exception("Email already verified");

            await SendVerificationEmailAsync(user);
        }

        public void EnsureEmailVerified(User user)
        {
            if (!user.IsEmailVerified)
                throw new Exception("Please verify your email first");
        }
    }

}
