﻿using System;
using System.Linq;
using System.Web.Http;

namespace AuctionBayWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "UsersApi",
                routeTemplate: "api/users/{action}",
                defaults: new
                {
                    controller = "users"
                }
            );

            config.Routes.MapHttpRoute(
                name: "ProductsApi",
                routeTemplate: "api/products/{action}/{id}",
                defaults: new
                {
                    controller = "products",
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "OffersApi",
                routeTemplate: "api/offers/{action}/{id}",
                defaults: new
                {
                    controller = "offers",
                    id = RouteParameter.Optional
                }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
