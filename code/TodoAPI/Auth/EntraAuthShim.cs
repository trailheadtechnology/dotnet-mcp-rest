using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TodoAPI.Auth;

/// <summary>
/// Bridges the gap between what MCP clients expect from an authorization server and what Entra ID provides.
/// MCP clients (Claude Code, MCP Inspector, mcp-remote) expect:
///   1. OAuth server metadata at /.well-known/oauth-authorization-server
///   2. Dynamic client registration (Entra has none, so we hand back the pre-registered client id)
///   3. The RFC 8707 "resource" parameter to be accepted (Entra's v2.0 endpoints reject it, so we strip it)
/// </summary>
public static class EntraAuthShim
{
    public static IEndpointRouteBuilder MapEntraAuthShim(this IEndpointRouteBuilder app, string serverUrl)
    {
        var group = app.MapGroup("").ExcludeFromDescription(); // keep these out of Swagger

        // 1. Authorization server metadata (RFC 8414). Points at our proxies below, which forward to Entra.
        group.MapGet("/.well-known/oauth-authorization-server", ServerMetadata);
        group.MapGet("/.well-known/openid-configuration", ServerMetadata);

        // 2. Dynamic client registration (RFC 7591). Every "registration" gets the same pre-registered client id.
        group.MapPost("/register", (
            [FromBody] RegistrationRequest request,
            IOptions<EntraOptions> options) =>
        {
            return Results.Created("/register", new
            {
                client_id = options.Value.ClientId,
                client_name = request.client_name,
                redirect_uris = request.redirect_uris,
                grant_types = new[] { "authorization_code", "refresh_token" },
                response_types = new[] { "code" },
                token_endpoint_auth_method = "none",
            });
        });

        // 3a. Authorize proxy: drop "resource", redirect the browser to Entra.
        group.MapGet("/authorize", (HttpRequest request, IOptions<EntraOptions> options) =>
        {
            var query = request.Query
                .Where(kv => kv.Key != "resource")
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value.ToString())}");
            return Results.Redirect($"{options.Value.AuthorizeEndpoint}?{string.Join("&", query)}");
        });

        // 3b. Token proxy: drop "resource", forward the form post to Entra, relay the response.
        group.MapPost("/token", async (
            HttpRequest request,
            IOptions<EntraOptions> options,
            IHttpClientFactory httpClientFactory,
            CancellationToken ct) =>
        {
            var form = await request.ReadFormAsync(ct);
            var fields = form
                .Where(kv => kv.Key != "resource")
                .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()));

            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync(options.Value.TokenEndpoint, new FormUrlEncodedContent(fields), ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
        });

        return app;

        IResult ServerMetadata(IOptions<EntraOptions> options) => Results.Json(new
        {
            issuer = options.Value.Issuer,
            authorization_endpoint = $"{serverUrl}/authorize",
            token_endpoint = $"{serverUrl}/token",
            registration_endpoint = $"{serverUrl}/register",
            jwks_uri = $"https://login.microsoftonline.com/{options.Value.TenantId}/discovery/v2.0/keys",
            scopes_supported = new[] { options.Value.Scope },
            response_types_supported = new[] { "code" },
            grant_types_supported = new[] { "authorization_code", "refresh_token" },
            code_challenge_methods_supported = new[] { "S256" },
            token_endpoint_auth_methods_supported = new[] { "none" },
        });
    }

    public sealed record RegistrationRequest(string? client_name, string[]? redirect_uris);
}
