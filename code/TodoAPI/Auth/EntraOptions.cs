namespace TodoAPI.Auth;

/// <summary>Bound from the "Entra" section of appsettings.json.</summary>
public sealed class EntraOptions
{
    public const string SectionName = "Entra";

    public string TenantId { get; set; } = "";
    public string ClientId { get; set; } = "";

    /// <summary>The scope exposed on the app registration ("Expose an API" in the portal).</summary>
    public string ScopeName { get; set; } = "Todos.Access";

    public string Issuer => $"https://login.microsoftonline.com/{TenantId}/v2.0";
    public string AuthorizeEndpoint => $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize";
    public string TokenEndpoint => $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token";
    public string Scope => $"api://{ClientId}/{ScopeName}";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(TenantId) || string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException(
                "OAuth demo is enabled but Entra:TenantId / Entra:ClientId are empty in appsettings.json.");
        }
    }
}
