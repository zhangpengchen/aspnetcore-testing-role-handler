﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCore.Testing.RoleHandler
{
    public class CustomRoleWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        public CustomRoleHandlerHeaderConfig RoleConfig { get; } = new CustomRoleHandlerHeaderConfig();

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            if (RoleConfig.AnonymousRequest)
            {
                return;
            }

            string json = JsonConvert.SerializeObject(RoleConfig);

            string base64Json = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bypass", base64Json);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(x => x.DefaultScheme = CustomRoleHandler.AuthenticationScheme)
                    .AddScheme<CustomRoleHandlerOptions, CustomRoleHandler>(CustomRoleHandler.AuthenticationScheme,
                        x => { });
            });
        }
    }
}