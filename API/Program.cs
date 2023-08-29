
using API.Data;
using API.Extensions;
using API.MiddleWare;
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
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var cotntext = services.GetRequiredService<DataContext>();
    await cotntext.Database.MigrateAsync();
    await Seed.seedUsers(cotntext);
}
catch (Exception eX)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(eX, "an error occured");

}



app.Run();
