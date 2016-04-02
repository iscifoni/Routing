
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Internal;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class RoutingDispatcher : IRouteHandler, IRouter
    {
        private readonly IEndpointHandlerFactory _handlerFactory;
        private readonly DefaultEndpointSelector _selector;

        public RoutingDispatcher(IReadOnlyList<EndpointDescriptor> endpoints, IEndpointHandlerFactory handlerFactor)
        {
            _selector = new DefaultEndpointSelector(endpoints);
            _handlerFactory = handlerFactor;
        }

        public RequestDelegate GetRequestHandler(HttpContext httpContext, RouteData routeData)
        {
            var endpoint = _selector.Select(httpContext, routeData);
            if (endpoint == null)
            {
                return null;
            }

            var handler = _handlerFactory.CreateHandler(httpContext, routeData, endpoint);
            return handler;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            context.Handler = GetRequestHandler(context.HttpContext, context.RouteData);
            return TaskCache.CompletedTask;
        }
    }
}
