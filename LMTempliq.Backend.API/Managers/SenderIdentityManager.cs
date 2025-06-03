using LMTempliq.Backend.API.Models;
using Microsoft.Extensions.Options;

namespace LMTempliq.Backend.API.Managers;

public class SenderIdentityManager
{
    private readonly ILogger<SenderIdentityManager> _logger;
    private readonly IOptions<List<SenderIdentity>> _senderIdentityOptions;
    
    private List<SenderIdentity> _senderIdentities = new List<SenderIdentity>();
    
    public SenderIdentityManager(ILogger<SenderIdentityManager> logger, IOptions<List<SenderIdentity>> senderIdentityOptions)
    {
        _logger = logger;
        _senderIdentityOptions = senderIdentityOptions;
    }
    
    private void LoadAdressesAsync()
    {
        _logger.LogInformation("Loading addresses...");
        
        _senderIdentities = _senderIdentityOptions.Value;
        
        _logger.LogInformation($"{_senderIdentities.Count} adresses loaded successfully.");
    }
    
    public SenderIdentity GetSenderIdentity(string identityName)
    {
        if (_senderIdentities.Count == 0)
        {
            LoadAdressesAsync();
        }

        var identity = _senderIdentities.FirstOrDefault(i => i.Email.Equals(identityName, StringComparison.OrdinalIgnoreCase));
        
        if (identity == null)
        {
            _logger.LogWarning("Sender identity '{IdentityName}' not found.", identityName);
        }
        
        return identity;
    }
    
    
}