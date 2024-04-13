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

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
}).AddJwtBearer(options =>
{
    options.Authority = Environment.GetEnvironmentVariable("AUTH_DOMAIN");
    options.Audience = "https://localhost:7074/swagger/index.html/api";

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
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
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
                    AuthorizationUrl = new System.Uri(Environment.GetEnvironmentVariable("AUTH_DOMAIN")),
                    TokenUrl = new System.Uri(Environment.GetEnvironmentVariable("AUTH_DOMAIN"))
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
            new string[] {}
        }
    });
    });

builder.Services.AddDbContext<ConvertToolDbContext>(options => options.UseSqlServer(
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    )
);

builder.Services.AddRazorPages();

builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseRouting();

//app.UseAuthorization();
//app.UseAuthentication();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapFallbackToPage("/login");
});


var env = app.Environment;

if (env.IsDevelopment())
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

app.MapControllers();

app.Run();
