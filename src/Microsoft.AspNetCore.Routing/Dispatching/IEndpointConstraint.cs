
namespace Microsoft.AspNetCore.Routing.Dispatching
{
    public interface IEndpointConstraint
    {
        int Order { get; }

        bool Accept(EndpointSelectorContext context);
    }
}
