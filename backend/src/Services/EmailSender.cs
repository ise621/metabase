using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Metabase.Services;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "About to send email to `{Recipient}` with subject `{Subject}` and body `{Body}`")]
    public static partial void AboutToSendEmail(
        this ILogger logger,
        (string name, string address) Recipient,
        string Subject,
        string Body
    );
}

public sealed class EmailSender(
    string smtpHost,
    int smtpPort,
    ILogger<EmailSender> logger
    )
        : IEmailSender
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly string _smtpHost = smtpHost;
    private readonly int _smtpPort = smtpPort;

    public Task SendAsync(
        (string name, string address) recipient,
        string subject,
        string body
    )
    {
        _logger.AboutToSendEmail(recipient, subject, body);
        var message = new MimeMessage();
        message.From.Add(
            new MailboxAddress(
                "Metabase",
                "metabase@buildingenvelopedata.org"
            )
        );
        message.To.Add(
            new MailboxAddress(
                recipient.name,
                recipient.address
            )
        );
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };
        using (var client = new SmtpClient())
        {
            client.Connect(
                _smtpHost,
                _smtpPort,
                SecureSocketOptions.StartTlsWhenAvailable
            );
            // client.Authenticate("joey", "password");
            client.Send(message);
            client.Disconnect(true);
        }

        return Task.FromResult(0);
    }
}