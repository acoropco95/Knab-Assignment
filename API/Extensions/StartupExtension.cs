using Knab.Assignment.API.Providers;
using Knab.Assignment.Database.Data;
using Knab.Assignment.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Knab.Assignment.API.Extensions
{
    public static class StartupExtension
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityBuilder builder = services.AddIdentityCore<User>(option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 4;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddSignInManager<SignInManager<User>>();

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            var securityKey = configuration.GetSection("AppSettings:JWTSecurityKey").Value;
            if (string.IsNullOrEmpty(securityKey))
                throw new Exception("Security key is missing in appsettings.json");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }
    }
}
