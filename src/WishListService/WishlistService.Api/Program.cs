using Shared.Extensions;
using WishlistService.Application;
using WishlistService.Infrastructure;
using WishlistService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWishListApplicationDependencyInjection(builder.Configuration);
builder.Services.AddWishListInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("WishListService.Api");

builder.Services.AddHttpClients();

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
