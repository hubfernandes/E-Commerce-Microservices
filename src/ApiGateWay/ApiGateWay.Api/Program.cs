using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.AuthShared;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

//builder.Services.AddHttpClient("AuthService", client =>
//{
//    client.BaseAddress = new Uri("http://localhost:5089/");
//    client.Timeout = TimeSpan.FromSeconds(30);
/*
- Deleting AddHttpClient("AuthService", ...) does not affect Ocelot's functionality because Ocelot manages its own HTTP clients internally.
- This only affects explicit HTTP calls that would have used httpClientFactory elsewhere in your API Gateway.*/
//});

builder.Services.AddSharedJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("Api GateWay");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseOcelot().Wait();
app.MapControllers();

app.Run();
