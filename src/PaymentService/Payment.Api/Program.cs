using Payment.Application;
using Payment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPaymentInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection(builder.Configuration);
builder.Services.AddHttpClient("OrderService", client => // for IHttpClientFactory
{
    client.BaseAddress = new Uri("http://localhost:5193/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


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
