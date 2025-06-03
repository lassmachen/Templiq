using System.Text.Json.Nodes;
using LMTempliq.Backend.API.Managers;
using LMTempliq.Backend.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using Scriban;

namespace LMTempliq.Backend.API.Controllers;

[ApiController]
[Route("email")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;
    private readonly MailManager _mailManager;
    private readonly TemplateManager _templateManager;
    
    public EmailController(ILogger<EmailController> logger, MailManager mailManager, TemplateManager templateManager)
    {
        _logger = logger;
        _mailManager = mailManager;
        _templateManager = templateManager;
    }
    
    [HttpPost("plain")]
    public async Task<IActionResult> SendEmail([FromQuery] string identity, string toName, string toMail, string subject)
    {
        if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(toMail) || string.IsNullOrWhiteSpace(subject))
        {
            _logger.LogError("Invalid email parameters provided.");
            return new BadRequestObjectResult("Invalid email parameters provided.");
        }

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        
        await _mailManager.SendEmailAsync(identity, toName, toMail, subject, body);
        
        return new OkResult();
    }
    
    [HttpPost("template")]
    public async Task<IActionResult> SendEmailWithTemplate([FromQuery] string identity, string toName, string toMail, string subject, string template)
    { 
        if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrWhiteSpace(toName) || string.IsNullOrWhiteSpace(toMail) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(template))
        {
            _logger.LogError("Invalid email parameters provided.");
            return new BadRequestObjectResult("Invalid email parameters provided.");
        }
        
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        JObject? jObject = JObject.Parse(body);
        
        _logger.LogDebug(jObject.ToString());

        var rendered = await _templateManager.RenderTemplate(template, jObject);
        
        _logger.LogDebug(rendered);
        
        await _mailManager.SendEmailAsync(identity, toName, toMail, subject, rendered);

        return new OkResult();
    }
}