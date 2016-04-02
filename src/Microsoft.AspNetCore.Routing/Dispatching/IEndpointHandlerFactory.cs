
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public interface IEndpointHandlerFactory
    {
        RequestDelegate CreateHandler(HttpContext httpContext, RouteData routeData, EndpointDescriptor endpoint);
    }
}
