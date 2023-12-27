using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SimpleAPI;
using SimpleAPI.Data;
using SimpleAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SimpleAPI.Services;
using SimpleAPI.Services.Impl;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Database Config
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<TokenService, TokenServiceImpl>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var tokenSettings = builder.Configuration.GetSection("Token");
        var secretKey = tokenSettings.GetValue<string>("SecretKey");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenSettings.GetValue<string>("Issuer"),
            ValidAudience = tokenSettings.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden; // 403
                context.Fail("Authentication failed");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<BaseRoute>(builder.Configuration.GetSection("BaseRoute"));
var baseroute = builder.Configuration.GetSection("BaseRoute").Get<BaseRoute>();
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<BaseRoute>>().Value);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (baseroute.BlockRoutingWithoutBase)
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/" + baseroute.Base.Split('/')[1]) || context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next();
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Not Found");
        }
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{baseroute.Base}" } };
        });
    });
    app.UseSwaggerUI();
}

app.UsePathBase(new PathString(baseroute.Base));

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
