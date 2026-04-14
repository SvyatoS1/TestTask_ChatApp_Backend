using Microsoft.AspNetCore.SignalR;
using TestTask_ChatApp.Data;
using TestTask_ChatApp.Data.Models;
using TestTask_ChatApp.DTOs;
using TestTask_ChatApp.Services;

namespace TestTask_ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _db;
        private readonly ISentimentService _sentiment;
        private readonly ILogger<ChatHub> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ChatHub(
            ChatDbContext db,
            ISentimentService sentiment,
            IServiceScopeFactory scopeFactory,
            ILogger<ChatHub> logger
            )
        {
            _db = db;
            _sentiment = sentiment;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task JoinRoom(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            _logger.LogInformation("User {ConnectionId} joined room {Room}", Context.ConnectionId, room);
        }
        public async Task LeaveRoom(string room)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            _logger.LogInformation("User {ConnectionId} left room {Room}", Context.ConnectionId, room);
        }
        public async Task SendMessage(string username, string content, string room = "general")
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(content) || content.Length > 2000)
            {
                _logger.LogWarning("Invalid message from {ConnectionId}: empty username or content", Context.ConnectionId);
                return;
            }

            var message = new ChatMessage
            {
                Username = username,
                Content = content,
                Room = room,
                SentAt = DateTime.UtcNow
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            var dto = new MessageDto
            (
                message.Id,
                message.Username,
                message.Content,
                message.Room,
                message.SentAt,
                Sentiment: null
            );

            _logger.LogInformation("Message from {Username} in {Room}: {Content}", username, room, content);
            await Clients.Group(room).SendAsync("ReceiveMessage", dto);

            _ = Task.Run(async () =>
            {
                try
                {
                    var (label, score) = await _sentiment.AnalyzeSentimentAsync(message.Content);

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();

                    var result = new SentimentResult
                    {
                        MessageId = message.Id,
                        Label = label,
                        ConfidenceScore = score
                    };
                    db.SentimentResults.Add(result);
                    await db.SaveChangesAsync();

                    await hubContext.Clients.Group(room).SendAsync("SentimentUpdate", new SentimentUpdateDto(
                        message.Id, label, score
                    ));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sentiment analysis failed for message {Id}", message.Id);
                }
            });
        }
    }
}
