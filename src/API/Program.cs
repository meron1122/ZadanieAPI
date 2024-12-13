using Core.Interfaces;
using Infrastructure.Repository;
using Infrastructure.Wrapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EmployeeTimeManagement API",
        Version = "v1"
    });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Enter your username and password for Basic Authentication."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(provider => new DbConnection(connectionString));
builder.Services.AddSingleton<IDapperWrapper, DapperWrapper>();
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<ITimeEntryRepo, TimeEntryRepo>();

var app = builder.Build();
app.UseMiddleware<API.ExceptionHanlder.ExceptionHandler>();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmployeeTimeManagement API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/swagger") &&
                       !context.Request.Path.StartsWithSegments("/v1/swagger.json"), subApp =>
                       {
                           subApp.UseAuthentication();
                           subApp.UseAuthorization();
                       });

app.UseCors("AllowAllOrigins");
app.MapControllers();
app.Run();
