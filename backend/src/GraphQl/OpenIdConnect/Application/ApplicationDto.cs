namespace Metabase.GraphQl.OpenIdConnect.Application;

public class ApplicationDto
{
    public string? Id { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
    public string DisplayName { get; }
    public string Permissions { get; }

    public ApplicationDto(string id, string clientId, string clientSecret, string displayName, string permissions)
    {
        Id = id;
        ClientId = clientId;
        ClientSecret = clientSecret;
        DisplayName = displayName;
        Permissions = permissions;
    }
}