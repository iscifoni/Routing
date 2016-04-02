
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public interface IEndpointSelector
    {
        EndpointDescriptor Select(HttpContext httpContext, RouteData routeData);
    }
}
