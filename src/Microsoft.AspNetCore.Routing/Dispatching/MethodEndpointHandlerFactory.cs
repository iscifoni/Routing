using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class MethodEndpointHandlerFactory : IEndpointHandlerFactory
    {
        private static readonly object[] EmptyArguments = new object[0];

        private readonly Type _handlerType;
        private readonly ObjectFactory _handlerFactory;
        private readonly KeyValuePair<MethodEndpointDescriptor, RequestDelegate>[] _handlers;

        public MethodEndpointHandlerFactory(Type handlerType, IReadOnlyList<MethodEndpointDescriptor> endpoints)
        {
            _handlerType = handlerType;
            _handlerFactory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

            _handlers = new KeyValuePair<MethodEndpointDescriptor, RequestDelegate>[endpoints.Count];
            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = new KeyValuePair<MethodEndpointDescriptor, RequestDelegate>(
                    endpoints[i],
                    CreateHandler(endpoints[i]));
            }
        }

        public RequestDelegate CreateHandler(
            HttpContext httpContext,
            RouteData routeData,
            EndpointDescriptor endpoint)
        {
            for (var i = 0; i < _handlers.Length; i++)
            {
                if (object.ReferenceEquals(_handlers[i].Key, endpoint))
                {
                    return _handlers[i].Value;
                }
            }

            return null;
        }

        private RequestDelegate CreateHandler(MethodEndpointDescriptor endpoint)
        {
            var httpContextParameter = Expression.Parameter(typeof(HttpContext), "httpContext");

            var parameters = new[] { httpContextParameter };

            return 
                Expression.Lambda<RequestDelegate>(
                    Expression.Call(
                        Expression.Convert(
                            Expression.Invoke(
                                Expression.Constant(_handlerFactory),
                                Expression.MakeMemberAccess(httpContextParameter, typeof(HttpContext).GetTypeInfo().GetDeclaredProperty(nameof(HttpContext.RequestServices))),
                                Expression.Constant(EmptyArguments)),
                            _handlerType),
                        endpoint.Method,
                        httpContextParameter),
                    parameters)
                .Compile();
        }
    }
}
