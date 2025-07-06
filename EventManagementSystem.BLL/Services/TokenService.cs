using EventManagementSystem.API.DTOs.AuthDtos;
using EventManagementSystem.BLL.Healpers;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.BLL.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly GenericRepository<User> _userRepository;

        public TokenService(GenericRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // ✅ تسجيل الدخول
        public async Task<LoginResponseDto?> Authunticate(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var user = await _userRepository
                .FindByCondition(x => x.Email == request.Email)
                .FirstOrDefaultAsync();

            if (user == null || !PasswordHashHandler.VerifyPassword(request.Password, user.PasswordHash!))
                return null;

            var accessToken = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                Email = user.Email,
                ExpiredIn = GetTokenExpiryInSeconds()
            };
        }

        // ✅ تسجيل مستخدم جديد + إصدار توكن تلقائي
        public async Task<LoginResponseDto?> RegisterAsync(RegisterUserDto request)
        {
            var existingUser = await _userRepository
                .FindByCondition(x => x.Email == request.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
                return null;

            var hashedPassword = PasswordHashHandler.HashPassword(request.PasswordHash);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Phone = request.Phone,
                IsActive = true
            };

            await _userRepository.AddAsync(newUser);

            // تسجيل الدخول مباشرة بعد التسجيل
            return await Authunticate(new LoginRequestDto
            {
                Email = request.Email,
                Password = request.PasswordHash
            });
        }

        // 🛡️ إنشاء التوكن JWT
        private string GenerateJwtToken(User user)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var expiryMinutes = int.Parse(_configuration["JwtConfig:DurationInMins"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ضروري لبعض الأماكن
                new Claim("id", user.Id.ToString()) // ✅ علشان JWTReader يشتغل
            };

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ⏳ احسب عدد الثواني حتى انتهاء التوكن
        private int GetTokenExpiryInSeconds()
        {
            var expiryMinutes = int.Parse(_configuration["JwtConfig:DurationInMins"]);
            return (int)TimeSpan.FromMinutes(expiryMinutes).TotalSeconds;
        }
    }
}
