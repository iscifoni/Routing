// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Dispatching;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace RoutingSample.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<PetStore.PetStoreContext>(options =>
                {
                    options.UseInMemoryDatabase();
                });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, ObjectPoolProvider poolProvider)
        {
            var routeBuilder = new TreeRouteBuilder(loggerFactory);

            var template = TemplateParser.Parse("pet");
            var endpoints = new List<MethodEndpointDescriptor>()
            {
                new MethodEndpointDescriptor()
                {
                    Constraints = new List<IEndpointConstraint>()
                    {
                        new HttpMethodEndpointConstraint("POST"),
                    },
                    Method = typeof(PetStore.PetHandler).GetTypeInfo().GetMethod(nameof(PetStore.PetHandler.AddPet)),
                    RouteValues = new RouteValueDictionary(),
                },
            };

            var handlerFactory = new MethodEndpointHandlerFactory(typeof(PetStore.PetHandler), endpoints);

            routeBuilder.Add(new TreeRouteMatchingEntry()
            {
                RouteTemplate = template,
                Precedence = RoutePrecedence.ComputeMatched(template),
                Target = new RoutingDispatcher(endpoints, handlerFactory),
                TemplateMatcher = new TemplateMatcher(template, new RouteValueDictionary()),
            });



            template = TemplateParser.Parse("pet/{id}");
            endpoints = new List<MethodEndpointDescriptor>()
            {
                new MethodEndpointDescriptor()
                {
                    Constraints = new List<IEndpointConstraint>()
                    {
                        new HttpMethodEndpointConstraint("GET"),
                    },
                    Method = typeof(PetStore.PetHandler).GetTypeInfo().GetMethod(nameof(PetStore.PetHandler.FindById)),
                    RouteValues = new RouteValueDictionary(),
                },
            };

            handlerFactory = new MethodEndpointHandlerFactory(typeof(PetStore.PetHandler), endpoints);

            routeBuilder.Add(new TreeRouteMatchingEntry()
            {
                RouteTemplate = template,
                Precedence = RoutePrecedence.ComputeMatched(template),
                Target = new RoutingDispatcher(endpoints, handlerFactory),
                TemplateMatcher = new TemplateMatcher(template, new RouteValueDictionary()),
            });

            routeBuilder.Add(new TreeRouteLinkGenerationEntry()
            {
                Binder = new TemplateBinder(
                    UrlEncoder.Default,
                    poolProvider.Create<UriBuildingContext>(new UriBuilderContextPooledObjectPolicy(UrlEncoder.Default)),
                    template,
                    new RouteValueDictionary()),
                GenerationPrecedence = RoutePrecedence.ComputeGenerated(template),
                Name = "FindById",
                Template = template,
                Target = new RoutingDispatcher(endpoints, handlerFactory),
                RequiredLinkValues = new RouteValueDictionary(),
            });


            app.UseRouter(routeBuilder.Build());
        }

        private void CreateDatabase(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = services.GetRequiredService<PetStore.PetStoreContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultHostingConfiguration(args)
                .UseIISPlatformHandlerUrl()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}