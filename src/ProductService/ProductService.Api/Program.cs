using Microsoft.Extensions.Options;
using ProductService.Application;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Services;
using Shared.AuthShared;
using Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructurefDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection(builder.Configuration);

builder.Services.AddHttpClients();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("ProductService.Api");

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UserSharedMiddleWare();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value); // for localization
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
