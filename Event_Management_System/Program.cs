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

namespace Event_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();

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

            builder.Services.AddValidatorsFromAssemblyContaining<EmailQueueValidator>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
