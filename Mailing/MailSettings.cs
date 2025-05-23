namespace Mailing;

public class MailSettings
{
    public required string Server { get; set; }
    public int Port { get; set; }
    public required string SenderFullName { get; set; }
    public required string SenderEmail { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public MailSettings()
    {
    }

    public MailSettings(string server, int port, string senderFullName, string senderEmail, string userName, string password)
    {
        Server = server;
        Port = port;
        SenderFullName = senderFullName;
        SenderEmail = senderEmail;
        UserName = userName;
        Password = password;
    }
}