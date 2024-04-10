using CodeConverterTool.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = Environment.GetEnvironmentVariable("OPTIONS_AUTHORITY");
    options.Audience = "https://localhost:7074/swagger/index.html/api";
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ConvertToolDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConvertToolConnectionString")
    )
);

builder.Services.AddRazorPages();

var app = builder.Build();

//app.UseRouting();
//app.UseAuthorization();

/*app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});*/


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
