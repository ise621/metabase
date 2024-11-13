namespace Metabase.GraphQl.Users;

public sealed class TwoFactorAuthentication(
    bool hasAuthenticator,
    bool isEnabled,
    bool isMachineRemembered,
    int recoveryCodesLeftCount
    )
{
    public bool HasAuthenticator { get; } = hasAuthenticator;
    public bool IsEnabled { get; } = isEnabled;
    public bool IsMachineRemembered { get; } = isMachineRemembered;
    public int RecoveryCodesLeftCount { get; } = recoveryCodesLeftCount;
}