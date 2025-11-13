using TodoMcp.Shared;

namespace Microsoft.AspNetCore.Builder;

public static class EndpointRegistrationExtension
{
    public static void MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var endpointDefinitions = typeof(Program).Assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();

        foreach (var endpointDefinition in endpointDefinitions)
        {
            endpointDefinition.MapEndpoints(endpoints);
        }
    }
}
