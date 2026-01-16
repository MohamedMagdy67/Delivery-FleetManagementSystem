using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemDTOS.AuthenticationDTOS;
using SystemContext.SystemDbContext;
using SystemModel.Entities;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
namespace ServiceLayer.AuthenticationServices
{
    public class AuthService:IAuthService
    {
        public readonly DelivryDB _context;
        private readonly JwtSettings _jwtSettings;
        private readonly EmailVerificationService _emailVerificationService;
        public AuthService(DelivryDB context, IOptions<JwtSettings> jwtSettings,EmailVerificationService emailVerificationService)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _emailVerificationService = emailVerificationService;    
        }
        public async Task<UserDTO> RegisterAsync(RegisterDTO dto)
        {
if(string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Role.ToString()))
        {
                throw new Exception("Invaild User Details");
        }
            var user = _context.Users.FirstOrDefault(u => u.Email== dto.Email);
            if (user != null) { throw new Exception("This User Is Exist"); }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var User = new User()
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                Role = dto.Role,
                IsActive = true

            };
            if(dto.Phone != null)
                {
                    User.Phone = dto.Phone;
                }
            _context.Users.Add(User);  
            _context.SaveChanges();
            await _emailVerificationService.SendVerificationEmailAsync(User);

            UserDTO userDTO = new UserDTO() {Name = User.Name ,Phone = User.Phone,Email = User.Email,Role = User.Role,CreatedAt = User.CreatedAt };
            return userDTO;
        }
        public AuthDTO Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid Email Or Password");

            _emailVerificationService.EnsureEmailVerified(user);

            List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Role,user.Role.ToString()),
            new Claim("userName", user.Name)
        };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expire = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expire,
                signingCredentials: creds
            );

            return new AuthDTO { Token = new JwtSecurityTokenHandler().WriteToken(token), ExpireDate = expire };
        }

    }
}

