using Currency_Conversion_Business.BusinessLayer;
using Currency_Conversion_Business.Interface;
using Currency_Conversion_API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Currency_Conversion_API.Validation;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<CurrencyFetchingService>();
builder.Services.AddScoped<IConverter, CurrencyConverter>();
builder.Host.UseSerilog(Log.Logger);
builder.Services.AddControllersWithViews()
           .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RequestValidation>());
IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false).Build();

builder.Services.AddSingleton(config);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:44351", "http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
