using MailKit.Net.Smtp;
using MimeKit;

namespace LMTempliq.Backend.API.Managers;

public class MailManager
{
    private readonly ILogger<MailManager> _logger;
    private readonly SenderIdentityManager _senderIdentityManager;
    private readonly TemplateManager _templateManager;
    
    public MailManager(ILogger<MailManager> logger, SenderIdentityManager senderIdentityManager, TemplateManager templateManager)
    {
        _logger = logger;
        _senderIdentityManager = senderIdentityManager;
        _templateManager = templateManager;
    }

    public async Task SendEmailAsync(string identityName, string toName, string toMail, string subject, string body)
    {
        var message = new MimeMessage();
        try
        {
            var senderIdentity = _senderIdentityManager.GetSenderIdentity(identityName);
            if (senderIdentity == null)
            {
                _logger.LogError($"Sender identity '{identityName}' not found.");
                return;
            }

            message.From.Add(new MailboxAddress(senderIdentity.Display, senderIdentity.Email));
            message.To.Add(new MailboxAddress(toName, toMail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(senderIdentity.Host, senderIdentity.Port, senderIdentity.Ssl);
            if (!string.IsNullOrEmpty(senderIdentity.Username) && !string.IsNullOrEmpty(senderIdentity.Password))
            {
                await client.AuthenticateAsync(senderIdentity.Username, senderIdentity.Password);
            }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully from '{senderIdentity.Email}' to '{toMail}' with subject '{subject}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email from '{identityName}' to '{toMail}' with subject '{subject}'.");
        }
        
    }
    
    public async Task SendEmailAsync(string identityName, string toName, string toMail, string subject, string body, string replyTo)
    {
        var message = new MimeMessage();
        try
        {
            var senderIdentity = _senderIdentityManager.GetSenderIdentity(identityName);
            if (senderIdentity == null)
            {
                _logger.LogError($"Sender identity '{identityName}' not found.");
                return;
            }

            message.From.Add(new MailboxAddress(senderIdentity.Display, senderIdentity.Email));
            message.To.Add(new MailboxAddress(toName, toMail));
            message.ReplyTo.Add(new MailboxAddress(replyTo, replyTo));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(senderIdentity.Host, senderIdentity.Port, senderIdentity.Ssl);
            if (!string.IsNullOrEmpty(senderIdentity.Username) && !string.IsNullOrEmpty(senderIdentity.Password))
            {
                await client.AuthenticateAsync(senderIdentity.Username, senderIdentity.Password);
            }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully from '{senderIdentity.Email}' to '{toMail}' with subject '{subject}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email from '{identityName}' to '{toMail}' with subject '{subject}'.");
        }
        
    }
}