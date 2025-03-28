using CartService.Application;
using CartService.Infrastructure;
using CartService.Infrastructure.Services;
using Shared.AuthShared;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCartApplicationDependencyInjection(builder.Configuration);
builder.Services.AddCartInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("CartService.Api");

builder.Services.AddHttpClients();

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
