using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Server.Entities;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(opt =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();