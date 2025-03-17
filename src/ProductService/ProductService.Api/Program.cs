using Microsoft.Extensions.Options;
using ProductService.Application;
using ProductService.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructurefDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection(builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value); // for localization

app.UserSharedMiddleWare();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
