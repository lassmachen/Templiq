using System.Dynamic;
using LMTempliq.Backend.API.Models;
using Newtonsoft.Json.Linq;

namespace LMTempliq.Backend.API.Managers;

public class TemplateManager
{
    private readonly ILogger<TemplateManager> _logger;
    
    public TemplateManager(ILogger<TemplateManager> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> RenderTemplate(string name, JObject model)
    {
        if (string.IsNullOrWhiteSpace(name) || model == null)
        {
            _logger.LogError("Template name cannot be null or empty, and model cannot be null.");
            throw new ArgumentException("Template name cannot be null or empty, and model cannot be null.", nameof(name));
        }
        
        var path = Path.Combine(Environment.CurrentDirectory, "Templates", $"{name}.html");
        
        if (!File.Exists(path))
        {
            _logger.LogError($"Template file '{path}' does not exist.");
            throw new FileNotFoundException($"Template file '{path}' does not exist.", path);
        }
        
        var body = await File.ReadAllTextAsync(path);
        
        var template = Scriban.Template.Parse(body);
        
        if (template.HasErrors)
        {
            _logger.LogError($"Template parsing errors: {string.Join(", ", template.Messages)}");
            throw new InvalidOperationException($"Template parsing errors: {string.Join(", ", template.Messages)}");
        }
        
        var data = model
            .Properties()
            .ToDictionary(p => p.Name.ToLower(), p => (object?)p.Value.ToString());

        
        var rendered = await template.RenderAsync(data);
        
        if (string.IsNullOrWhiteSpace(rendered))
        {
            _logger.LogError("Rendered template is empty.");
            throw new InvalidOperationException("Rendered template is empty.");
        }
        
        _logger.LogInformation($"Template '{name}' rendered successfully.");
        
        return rendered;
    }
}