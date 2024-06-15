using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Tables;
using ServerChat.Models;
using Microsoft.EntityFrameworkCore;

namespace ServerChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly Connection _context;

        public ChatController(Connection context)
        {
            _context = context;
        }

        [HttpGet("users/{currentUserId}")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers(int currentUserId)
        {
            var users = await _context.Userss
                .Where(u => u.UserID != currentUserId)
                .Select(u => new UserModel
                {
                    UserId = u.UserID,
                    UserName = $"{u.Surname} {u.Name}",
                    Avatar = Convert.ToBase64String(u.Photo ?? new byte[0]),
                    LastMessageText = _context.Messages
                        .Where(m => (m.UserId == currentUserId && m.SenderId == u.UserID) || (m.UserId == u.UserID && m.SenderId == currentUserId))
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => m.Text)
                        .FirstOrDefault(),
                    LastMessageTimestamp = _context.Messages
                        .Where(m => (m.UserId == currentUserId && m.SenderId == u.UserID) || (m.UserId == u.UserID && m.SenderId == currentUserId))
                        .OrderByDescending(m => m.Timestamp)
                        .Select(m => m.Timestamp)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Shorten long messages
            users.ForEach(u => {
                if (!string.IsNullOrEmpty(u.LastMessageText) && u.LastMessageText.Length > 10)
                {
                    u.LastMessageText = u.LastMessageText.Substring(0, 10) + "...";
                }
            });

            // Combine last message text and timestamp
            users.ForEach(u =>
            {
                u.LastMessage = string.IsNullOrEmpty(u.LastMessageText) ? string.Empty : $"{u.LastMessageText} ({u.LastMessageTimestamp?.ToString("dd/MM/yyyy HH:mm")})";
            });

            return Ok(users);
        }

        [HttpGet("messages/{userId}/{recipientId}")]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessages(int userId, int recipientId, [FromQuery] DateTime? lastTimestamp)
        {
            var query = _context.Messages.Where(m =>
                (m.UserId == userId && m.SenderId == recipientId) ||
                (m.UserId == recipientId && m.SenderId == userId));

            if (lastTimestamp.HasValue)
            {
                query = query.Where(m => m.Timestamp > lastTimestamp.Value);
            }

            var messages = await query
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageModel
                {
                    MessageId = m.MessageId,
                    UserId = m.UserId,
                    SenderId = m.SenderId,
                    Text = m.Text,
                    Timestamp = m.Timestamp
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<ActionResult<MessageModel>> SendMessage([FromBody] MessageModel model)
        {
            var user = await _context.Userss.FirstOrDefaultAsync(u => u.UserID == model.UserId);
            var sender = await _context.Userss.FirstOrDefaultAsync(u => u.UserID == model.SenderId);

            if (user == null)
            {
                return BadRequest("User not found. Пожалуйста, выберите существующего пользователя.");
            }

            if (sender == null)
            {
                return BadRequest("Sender not found. Пожалуйста, выберите существующего отправителя.");
            }

            // Предотвращаем отправку сообщения самому себе
            if (model.UserId == model.SenderId)
            {
                return BadRequest("Вы не можете отправить сообщение самому себе.");
            }

            var message = new Message
            {
                UserId = model.UserId,
                SenderId = model.SenderId,
                Text = model.Text,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            model.MessageId = message.MessageId;
            model.Timestamp = message.Timestamp;

            return Ok(model);
        }
    }
}
