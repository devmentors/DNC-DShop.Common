using System;
using System.Net.Http;
using Autofac;
using DShop.Common.Options;
using Microsoft.Extensions.Configuration;
using RestEase;

namespace DShop.Common.RestEase
{
    public static class Extensions
    {
        public static void RegisterApiForwarder<T>(this ContainerBuilder containerBuilder, string fromSection)
        {
            var named = typeof(T).ToString();

            containerBuilder.Register(ct =>
            {
                var host = ct.Resolve<IConfiguration>().GetOptions<RestEaseOptions>(fromSection);
                var uriBuilder = new UriBuilder
                {
                    Scheme = host.Scheme,
                    Host = host.Host,
                    Port = host.Port
                };
                return new HttpClient() { BaseAddress = uriBuilder.Uri };

            }).Named<HttpClient>(named).SingleInstance();

            containerBuilder.Register(ct => 
            {
                var httpClient = ct.ResolveNamed<HttpClient>(named);
                return new RestClient(httpClient)
                { 
                    RequestQueryParamSerializer = new QueryParamSerializer() 
                }.For<T>();

            }).As<T>();
        }
    }
}