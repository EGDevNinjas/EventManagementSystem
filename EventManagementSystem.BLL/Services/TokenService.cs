using EventManagementSystem.API.DTOs.AuthDtos;
using EventManagementSystem.BLL.Healpers;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.DAL.Contexts;
using EventManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace EventManagementSystem.BLL.Services

{

	public class TokenService

	{
		private readonly IConfiguration _configuration;
		private readonly GenericRepository<User> _userRepository;

		public TokenService(GenericRepository<User> Data, IConfiguration configuration) {
			_userRepository = Data;
			_configuration = configuration;
		}


		public async Task<LoginResponseDto?> Authunticate(LoginRequestDto request)
		{
			if(string.IsNullOrWhiteSpace(request.Password)|| string.IsNullOrWhiteSpace(request.Email))
				return null;
			var userAccount = await _userRepository
				.FindByCondition(x => x.Email == request.Email)
				.FirstOrDefaultAsync();
			if (userAccount == null || !PasswordHashHandler.VerifyPassword(request.Password, userAccount.PasswordHash!))
				return null;
			var Issuer = _configuration["JwtConfig:Issuer"];
			var audience = _configuration["JwtConfig:Audience"];
			var key = _configuration["JwtConfig:Key"];
			var tokenValidMin = int.Parse(_configuration["JwtConfig:DurationInMins"]);
			var ExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidMin);
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, request.Email)
				}),
				Expires = ExpiryTimeStamp,
				Issuer = Issuer,
				Audience = audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
				,SecurityAlgorithms.HmacSha256),

			};
			var tokenHandeler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandeler.CreateToken(tokenDescriptor);
			var accessToken = tokenHandeler.WriteToken(securityToken);

			return new LoginResponseDto
			{
				AccessToken = accessToken,
				Email = request.Email,
				ExpiredIn = (int)ExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
			};
		}
	}
}
