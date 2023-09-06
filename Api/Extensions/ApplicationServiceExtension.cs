using Api.Services;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Api.Extensions;

public static class ApplicationServiceExtension
    {
            public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()        //WithOrigins("https://domini.com")
                .AllowAnyMethod()               //WithMethods(*Get", "POST")
                .AllowAnyHeader());             //WithHeaders(*accept", "content-type")
        });

            public static void AddAplicationServices(this IServiceCollection services)
            {
                services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IAuthorizationHandler, GlobalVerbRoleHandler>();
            }
    }
