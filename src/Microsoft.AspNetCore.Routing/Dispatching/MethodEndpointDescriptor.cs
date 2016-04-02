
using System.Reflection;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class MethodEndpointDescriptor : EndpointDescriptor
    {
        public MethodInfo Method { get; set; }
    }
}
