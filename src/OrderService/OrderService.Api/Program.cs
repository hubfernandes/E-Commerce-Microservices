using OrderService.Application;
using OrderService.Infrastructure;
using Payment.Application.BackgroundServices;
using Shared.AuthShared;
using Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection(builder.Configuration);
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("OrderService.Api");

builder.Services.AddHostedService<PaymentProcessedEventConsumer>();


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

builder.Services.AddHttpClient("CartService", client => client.BaseAddress = new Uri("http://localhost:5033"));
builder.Services.AddHttpClient("InventoryService", client => client.BaseAddress = new Uri("http://localhost:5140"));
builder.Services.AddHttpClient("PaymentService", client => client.BaseAddress = new Uri("http://localhost:5146"));



builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UserSharedMiddleWare();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
