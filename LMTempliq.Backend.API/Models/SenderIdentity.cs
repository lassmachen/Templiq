namespace LMTempliq.Backend.API.Models;

public class SenderIdentity
{
    public string Email { get; set; }
    public string Display { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool Ssl { get; set; } = true;
    public string Username { get; set; }
    public string Password { get; set; }

    public SenderIdentity(string email, string display, string host, int port, bool ssl, string username, string password)
    {
        Email = email;
        Display = display;
        Host = host;
        Port = port;
        Ssl = ssl;
        Username = username;
        Password = password;
    }
}