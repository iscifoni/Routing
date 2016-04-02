// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PetStore
{
    public class PetHandler
    {
        //public PetHandler(PetStoreContext dbContext)
        //{
        //    DbContext = dbContext;
        //}

        //public PetStoreContext DbContext { get; }

        public async Task FindById(HttpContext httpContext)
        {
            int id;
            if (!int.TryParse((string)httpContext.GetRouteValue("id"), out id))
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            //var pet = await DbContext.Pets
            //    .Include(p => p.Category)
            //    .Include(p => p.Images)
            //    .Include(p => p.Tags)
            //    .FirstOrDefaultAsync(p => p.Id == id);
            var pet = (Pet)null;
            if (pet == null)
            {
                httpContext.Response.StatusCode = 404;
                return;
            }

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";

            var text = JsonConvert.SerializeObject(pet);
            httpContext.Response.ContentLength = text.Length;
            await httpContext.Response.WriteAsync(text);
        }

        public async Task AddPet(HttpContext httpContext)
        {
            Pet pet;
            using (var reader = new JsonTextReader(new HttpRequestStreamReader(httpContext.Request.Body, Encoding.UTF8)))
            {
                pet = new JsonSerializer().Deserialize<Pet>(reader);
            }

            //DbContext.Pets.Add(pet);
            //await DbContext.SaveChangesAsync();

            var routeData = httpContext.GetRouteData();
            var virtualPathContext = new VirtualPathContext(httpContext, routeData.Values, new RouteValueDictionary(new { id = pet.Id }), "FindById");

            var router = routeData.Routers[0];
            var pathData = router.GetVirtualPath(virtualPathContext);
            if (pathData != null)
            {
                httpContext.Response.Headers["Location"] = pathData.VirtualPath;
            }

            httpContext.Response.StatusCode = 201;
            httpContext.Response.ContentType = "application/json";

            var text = JsonConvert.SerializeObject(pet);
            httpContext.Response.ContentLength = text.Length;
            await httpContext.Response.WriteAsync(text);
        }
    }
}