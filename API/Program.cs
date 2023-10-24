
using API.Data;
using API.Entities;
using API.Extensions;
using API.MiddleWare;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
// Swagger Service
builder.Services.AddSwaggerDocumentation();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseMiddleware<ExceptionMiddleWare>();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var cotntext = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await cotntext.Database.MigrateAsync();
    //cotntext.Connections.RemoveRange(cotntext.Connections);
    await cotntext.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    await Seed.seedUsers(userManager, roleManager);
}
catch (Exception eX)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(eX, "an error occured");

}



app.Run();
