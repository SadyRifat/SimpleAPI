using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SimpleAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

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
app.UsePathBase(new PathString(baseroute.Base));
app.UseRouting();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
