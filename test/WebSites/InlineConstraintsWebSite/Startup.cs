// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using InlineConstraintsWebSite.Constraints;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Constraints;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace InlineConstraints
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<RouteOptions>(
                routeOptions => routeOptions.ConstraintMap.Add(
                    "producttype",
                    typeof(ItemTypeConstraint)));

            services.Configure<RouteOptions>(
                routeOptions => routeOptions.ConstraintMap.Add(
                    "servicetype",
                    typeof(ItemTypeConstraint)));

            services.Configure<RouteOptions>(
                routeOptions =>
                {
                    if (routeOptions.ConstraintMap.ContainsKey("servicetype"))
                    {
                        routeOptions.ConstraintMap["servicetype"] =
                            typeof(ItemTypeUpperCaseConstraint);
                    }
                });

            // Add MVC services to the services container
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            var configuration = app.GetTestConfiguration();

            app.UseErrorReporter();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "producttype",
                    template: "producttype/{action}/{type:producttype(software)}",
                    defaults: new { controller = "inlineconstraints_software" });

                routes.MapRoute(
                    name: "servicetype",
                    template: "producttype/{action}/{type:servicetype(hardware)}",
                    defaults: new { controller = "inlineconstraints_hardware" });

                routes.MapRoute("StoreId",
                        "store/{action}/{id:guid?}",
                        defaults: new { controller = "InlineConstraints_Store" });

                routes.MapRoute("StoreLocation",
                        "store/{action}/{location:minlength(3):maxlength(10)}",
                        defaults: new { controller = "InlineConstraints_Store" },
                        constraints: new { location = new AlphaRouteConstraint() });

                // Used by tests for the 'exists' constraint.
                routes.MapRoute("areaExists-area", "area-exists/{area:exists}/{controller=Home}/{action=Index}");
                routes.MapRoute("areaExists", "area-exists/{controller=Home}/{action=Index}");
                routes.MapRoute("areaWithoutExists-area", "area-withoutexists/{area}/{controller=Home}/{action=Index}");
                routes.MapRoute("areaWithoutExists", "area-withoutexists/{controller=Home}/{action=Index}");

            });
        }
    }
}
