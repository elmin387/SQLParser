using Microsoft.Extensions.Options;
using SQLParse.Models;
using SQLParse.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));

builder.Services.AddHttpClient<ISQLFormatHandler, SQLFormatHandler>(client =>
{
    var apiSettings = builder.Configuration.GetSection("Config").Get<Config>();
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddScoped<ISQLFormatHandler, SQLFormatHandler>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
