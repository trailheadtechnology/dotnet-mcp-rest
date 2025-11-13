using Microsoft.AspNetCore.Routing;

namespace TodoMcp.Shared
{
    public interface IEndpoint
    {
        void MapEndpoints(IEndpointRouteBuilder endpoints);
    }
}