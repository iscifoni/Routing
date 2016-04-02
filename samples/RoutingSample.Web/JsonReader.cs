using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RoutingSample.Web
{
    public class JsonReader : IJsonReader
    {
        public T Read<T>(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
