using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Entities;
using Server.Services;
using Server.Utils;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<StockContext>(opt =>
{
    var platform = 
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)?"windows":"linux";
    if (builder.Environment.IsDevelopment())
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
    }
    else
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlserver_" + platform));
    }
});
builder.Services.AddTransient<IStockInfoService, StockInfoService>();
builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();

var tokenSecret = builder.Configuration.GetValue<string>("JWTToken");
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret))
    };
    opt.SaveToken = true;

    opt.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("token-expired", "true");
            }

            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapFallbackToFile("index.html");
// });
app.MapControllers();


JWTGenerator.Init(builder.Configuration.GetValue<string>("JWTToken"));

app.Run();