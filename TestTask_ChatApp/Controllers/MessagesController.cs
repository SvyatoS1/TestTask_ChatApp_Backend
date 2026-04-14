using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask_ChatApp.Data;
using TestTask_ChatApp.DTOs;

namespace TestTask_ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _db;
        public MessagesController(ChatDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IEnumerable<MessageDto>> GetMessages([FromQuery] string room = "general")
        {
            var messages = await _db.Messages
                .Where(m => m.Room == room)
                .OrderByDescending(m => m.SentAt)
                .Take(50)
                .Include(m => m.Sentiment)
                .Select(m => new MessageDto(
                    m.Id,
                    m.Username,
                    m.Content,
                    m.Room,
                    m.SentAt,
                    m.Sentiment == null ? null : new SentimentDto(m.Sentiment.Label, m.Sentiment.ConfidenceScore)
                ))
                .ToListAsync();
            return messages.OrderBy(m => m.SentAt);
        }
    }
}
