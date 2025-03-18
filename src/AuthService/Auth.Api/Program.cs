using Auth.Application;
using Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthInfrastructurefDependencyInjection(builder.Configuration);
builder.Services.AddAuthApplicationDependencyInjection(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtAuth();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await roleManager.EnsureRolesCreatedAsync();
}

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UserSharedMiddleWare();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
