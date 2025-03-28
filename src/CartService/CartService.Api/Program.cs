using CartService.Application;
using CartService.Infrastructure;
using Shared.AuthShared;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCartApplicationDependencyInjection(builder.Configuration);
builder.Services.AddCartInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("CartService.Api");

builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5279/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5089/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UserSharedMiddleWare();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
