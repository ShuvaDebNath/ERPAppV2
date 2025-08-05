using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Services;
using ERPApp.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;

namespace Auth.Infrastructure.Extensions;

public class AuthServiceInstaller : IServiceInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IPasswordHasher<AspNetUser>, PasswordHasher<AspNetUser>>();

        #region Model State Error Handle
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(new ModelStateDictionary());

                foreach (var error in errors)
                {
                    foreach (var msg in error.Value)
                    {
                        problemDetails.Errors.Add(error.Key, new[] { msg });
                    }
                }

                context.HttpContext.Items["ModelStateErrors"] = problemDetails;
                return new BadRequestObjectResult(problemDetails);
            };
        });

        #endregion

        #region JWT

        var jwtSettings = config.GetSection("Jwt");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
            };
        });

        #endregion
    }
}
