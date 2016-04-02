
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public class EndpointSelectorContext
    {
        public IReadOnlyList<EndpointDescriptor> Candidates { get; set; }

        public EndpointDescriptor CurrentCandidate { get; set; }

        public HttpContext HttpContext { get; set; }
        
        public RouteData RouteData { get; set; }
    }
}
