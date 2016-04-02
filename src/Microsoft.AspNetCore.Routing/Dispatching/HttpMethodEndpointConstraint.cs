
using System;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class HttpMethodEndpointConstraint : IEndpointConstraint
    {
        public HttpMethodEndpointConstraint(string httpMethod)
        {
            HttpMethod = httpMethod;
        }

        public string HttpMethod { get; }

        public int Order => 100;

        public bool Accept(EndpointSelectorContext context)
        {
            return string.Equals(HttpMethod, context.HttpContext.Request.Method, StringComparison.Ordinal);
        }
    }
}
