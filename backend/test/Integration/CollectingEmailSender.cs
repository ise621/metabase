using System.Collections.Generic;
using System.Threading.Tasks;
using Metabase.Services;

namespace Metabase.Tests.Integration;

public sealed class CollectingEmailSender
    : IEmailSender
{
    private readonly List<Email> _emails;

    public CollectingEmailSender()
    {
        _emails = [];
    }

    public IReadOnlyCollection<Email> Emails => _emails.AsReadOnly();

    public Task SendAsync(
        (string name, string address) recipient,
        string subject,
        string body
    )
    {
        _emails.Add(
            new Email
            (
                recipient,
                subject,
                body
            )
        );
        return Task.FromResult(0);
    }

    public void Clear()
    {
        _emails.Clear();
    }

    public sealed record Email(
        (string name, string address) Recipient,
        string Subject,
        string Body
    );
}