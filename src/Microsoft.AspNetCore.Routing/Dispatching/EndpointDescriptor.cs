
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class EndpointDescriptor
    {
        public IList<IEndpointConstraint> Constraints { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
    }
}
