using CodeConverterTool.Models;
using CodeConverterTool;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Auth0.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IdentityModel.AspNetCore.AccessTokenValidation;
using System.Text.Json;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

builder.Services.AddRazorPages().AddRazorPagesOptions(options => { options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()); });

builder.Services.AddControllers();

builder.Services.AddMvc().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})  
.AddJwtBearer(options =>
{
    options.Authority = Environment.GetEnvironmentVariable("AUTH_DOMAIN");
    options.Audience = Environment.GetEnvironmentVariable("AUDIENCE");

    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = c =>
        {
            c.NoResult();
            c.Response.StatusCode = 401;
            c.Response.ContentType = "text/plain";
            return c.Response.WriteAsync(c.Exception.ToString());
        },
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ConvertToolDbContext>(options => options.UseSqlServer(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    )
);

builder.Services.AddSession();
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "CodeConverTool", Version = "v1", Description = "Code Convert Tool Level Up 3" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows()
            {
                AuthorizationCode = new OpenApiOAuthFlow()
                {
                    AuthorizationUrl = new Uri(Environment.GetEnvironmentVariable("AUTHORIZE")!),
                    TokenUrl = new Uri(Environment.GetEnvironmentVariable("TOKEN")!)
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }});
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapFallbackToPage("/login");

app.MapControllers();

app.MapRazorPages();

app.Run();