using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Data.Tables;
using ServerChat.Models;

namespace ServerChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly Connection _context;

        public PasswordResetController(Connection context)
        {
            _context = context;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestModel model)
        {
            var user = await _context.Userss.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return BadRequest("Пользователь с указанным email не найден.");
            }

            user.PasswordResetToken = GenerateSixDigitCode(); // Генерируем код
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            // Отправить email с токеном пользователю
            EmailSend(user.Email, user.PasswordResetToken);

            return Ok("Инструкции по сбросу пароля отправлены на ваш email.");
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            var user = await _context.Userss.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && u.PasswordResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                return BadRequest("Недействительный или просроченный токен.");
            }

            user.Password = model.NewPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Пароль успешно сброшен.");
        }

        private void EmailSend(string email, string generatedCode)
        {
            string subject = "Код подтверждения и токен для сброса пароля";
            string body = $"Ваш код подтверждения: {generatedCode}\n" +
                          "Используйте этот код для подтверждения изменения пароля.\n\n" +
                          "Если вы не запрашивали изменение пароля, пожалуйста, проигнорируйте это письмо.";

            MailMessage message = new MailMessage("oddoneoddonov@gmail.com", email, subject, body);
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("oddoneoddonov@gmail.com", "siuj xxkp hquy uhcp");

            // Определяем хост и порт SMTP сервера в зависимости от почтового домена
            if (email.Contains("@yandex.ru"))
            {
                client.Host = "smtp.yandex.ru";
                client.Port = 587;
            }
            else if (email.Contains("@mail.ru"))
            {
                client.Host = "smtp.mail.ru";
                client.Port = 587;
            }
            else if (email.Contains("@gmail.com"))
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
            }
            client.EnableSsl = true; // Включаем SSL для безопасной передачи
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                // Логгирование ошибки отправки email
                Console.WriteLine("Не удалось отправить письмо. Ошибка: " + ex.Message);
            }
        }

        private string GenerateSixDigitCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
