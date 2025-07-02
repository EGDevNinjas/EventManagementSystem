using System.Configuration;
using EventManagementSystem.Core.EntityConfigs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL;
using EventManagementSystem.DAL.Contexts;
using EventManagementSystem.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using EventManagementSystem.Core.DTO_Validators;
using EventManagementSystem.BLL.Healpers;
using EventManagementSystem.Core.DTO_Validators.BookingValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventManagementSystem.BLL.Services;

namespace Event_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
                    ValidateIssuerSigningKey = true

				};
            });

            builder.Services.AddAuthorization();
			builder.Services.AddScoped<TokenService>();
			builder.Services.AddScoped<GenericRepository<User>>();
			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Config To Enable Auto Mapper 
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            // Adding Dependency Injection
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // عشان FluentValidation يقدر يلاقي الـ Validator classes اللي عملتها (زي UserValidator)
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<BookingValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingDTOValidator>();
            

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
            builder.Services.AddMemoryCache();

            var app = builder.Build();



            // Seed data
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    db.SeedData();
            //}

            app.UseMiddleware<ErrorHandlingMiddleware>();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
