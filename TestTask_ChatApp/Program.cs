using Microsoft.EntityFrameworkCore;
using TestTask_ChatApp.Data;
using TestTask_ChatApp.Hubs;
using TestTask_ChatApp.Services;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSQL")));

builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration["AzureSignalR:ConnectionString"]);

builder.Services.AddSingleton<ISentimentService, SentimentService>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(builder.Configuration["Frontend:Url"] ?? "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
