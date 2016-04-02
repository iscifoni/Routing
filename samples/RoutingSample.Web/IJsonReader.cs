
using Microsoft.AspNetCore.Http;

namespace RoutingSample.Web
{
    public interface IJsonReader
    {
        T Read<T>(HttpContext httpContext);
    }
}
