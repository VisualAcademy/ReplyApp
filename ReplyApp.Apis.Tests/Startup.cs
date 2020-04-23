using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReplyApp.Apis.Tests
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/token", async context =>
                {
                    var client = new HttpClient();

                    var discoveryDocument = await client.GetDiscoveryDocumentAsync("http://localhost:5050");

                    var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                        new ClientCredentialsTokenRequest
                        { 
                            Address = discoveryDocument.TokenEndpoint,
                            ClientId = "ReplyApp.Models.Tests",
                            ClientSecret = "ReplyApp.Apis",
                            Scope = "ReplyApp.Apis",
                        });

                    await context.Response.WriteAsync($"token: {tokenResponse.AccessToken}");

                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("안녕하세요.", System.Text.Encoding.UTF8);
                });
            });
        }
    }
}
