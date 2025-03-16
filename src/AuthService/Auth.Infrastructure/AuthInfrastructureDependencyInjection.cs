using Auth.Domain.Entities;
using Auth.Infrastructure.DbContext;
using Auth.Infrastructure.Interfaces;
using Auth.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.Infrastructure
{
    public static class AuthInfrastructureDependencyInjection
    {
        public static void AddAuthInfrastructurefDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            //Email 
            services.AddTransient<EmailTemplateService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // DI
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IGoogleService, GoogleService>();


            // Db
            services.AddDbContext<AuthDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("AuthConnection"),
                    sqlServerOption => sqlServerOption.EnableRetryOnFailure());
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
            })
          .AddEntityFrameworkStores<AuthDbContext>()
          .AddDefaultTokenProviders();


            // LockOut
            services.Configure<IdentityOptions>(op =>
            {
                op.Lockout.MaxFailedAccessAttempts = 5;
                op.Lockout.AllowedForNewUsers = true;
                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(40);
            });

        }

        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                };
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["GoogleAuth:ClientId"]!;
                options.ClientSecret = configuration["GoogleAuth:ClientSecret"]!;
            });


            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("PermissionPolicy", builder =>
            //    {
            //        builder.RequireClaim("Permission", "CanEditUsers");
            //    });
            //    options.AddPolicy("EmailPolicy", builder =>
            //    {
            //        builder.RequireClaim("EmailVerified", "falsse")
            //        ;
            //    });
            //});
        }
    }
}
