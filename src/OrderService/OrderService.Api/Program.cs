using CartService.Infrastructure.Services;
using OrderService.Application;
using OrderService.Infrastructure;
using Shared.AuthShared;
using Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrderInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection(builder.Configuration);
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("OrderService.Api");

builder.Services.AddHttpClients();

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
