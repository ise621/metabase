using System;

namespace Metabase.GraphQl.DataX;

public sealed class ResponseApproval(
    DateTime timestamp,
    string signature,
    string keyFingerprint,
    string query,
    string response
    )
        : IApproval
{
    public DateTime Timestamp { get; } = timestamp;
    public string Signature { get; } = signature;
    public string KeyFingerprint { get; } = keyFingerprint;
    public string Query { get; } = query;
    public string Response { get; } = response;
}