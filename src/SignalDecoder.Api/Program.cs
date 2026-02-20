using Microsoft.OpenApi.Models;
using SignalDecoder.Application.Service;
using SignalDecoder.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
   option.SwaggerDoc("v1", new OpenApiInfo
   {
       Version = "v1",
       Title = "Signal Decoder Api",
       Description = ""
   });
});

builder.Services.AddScoped<IDeviceGeneratorService, DeviceGeneratorService>();
builder.Services.AddScoped<ISignalSimulatorService, SignalSimulatorService>();
builder.Services.AddScoped<ISignalDecoderService, SignalDecoderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();