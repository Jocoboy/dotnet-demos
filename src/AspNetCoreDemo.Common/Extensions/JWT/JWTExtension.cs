using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions.JWT
{
    /// <summary>
    /// JWT扩展类
    /// </summary>
    public static class JWTExtension
    {
        /// <summary>
        /// JWT扩展方法
        /// </summary>
        /// <param name="builder"></param>
        public static void AddJWTExt(this WebApplicationBuilder builder)
        {
            #region JWT Token服务配置
            builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });
            #endregion
        }
    }
}
