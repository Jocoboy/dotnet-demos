﻿using AspNetCoreDemo.Model.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        public static JWTTokenOptions TokenOptions {  get; set; } = new JWTTokenOptions();

        /// <summary>
        /// JWT扩展方法
        /// </summary>
        /// <param name="builder"></param>
        public static void AddJWTExt(this WebApplicationBuilder builder)
        {
            #region JWT Token服务配置
            builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

            builder.Configuration.Bind("JWTTokenOptions", TokenOptions);
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenOptions.SecurityKey)),
                        ValidIssuer = TokenOptions.Issuer,
                        ValidAudience = TokenOptions.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });
            #endregion
        }

        public static string GetUserToken(CurrentUserDto user)
        {
            var json = JsonConvert.SerializeObject(user);
            var claimInfos = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return GenerateToken(claimInfos);
        }

        public static string GenerateToken(IDictionary<string, string> claimInfos)
        {
            byte[] securityKey = Encoding.UTF8.GetBytes(TokenOptions.SecurityKey);
            string algorithm = SecurityAlgorithms.HmacSha256;

            return GenerateToken(DictToClaims(claimInfos), securityKey, algorithm, TokenOptions.Issuer, TokenOptions.Audience, TokenOptions.AccessExpiration);
        }

        /// <summary>
        /// 字典数据转Token信息
        /// </summary>
        /// <param name="claimInfos"></param>
        /// <returns></returns>
        private static IEnumerable<Claim> DictToClaims(IDictionary<string, string> claimInfos)
        {
            List<Claim> claims = new List<Claim>();

            if (claimInfos.Any())
                claims.AddRange(claimInfos.Select(i => new Claim(i.Key, i.Value ?? "")));

            return claims;
        }

        public static string GenerateToken(IEnumerable<Claim> claims, byte[] securityKey, string algorithm, string issuer, string audience, int expires)
        {
            var key = new SymmetricSecurityKey(securityKey);
            var credentials = new SigningCredentials(key, algorithm);
            var jwtToken = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddMinutes(expires), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
