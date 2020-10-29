using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace Ocelot.APIGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "Gateway API", Version = "v1", Description = "# gateway api..." });
            });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).AddIdentityServerAuthentication("OrderService", options =>
            {
                options.Authority = "https://localhost:9080";//鉴权中心地址
                options.ApiName = "OrderApi";
                options.SupportedTokens = SupportedTokens.Both;
                options.ApiSecret = "orderApi secret";
                options.RequireHttpsMetadata = true;
            })
            .AddIdentityServerAuthentication("ProductService", options =>
            {
                options.Authority = "https://localhost:9080";//鉴权中心地址
                options.ApiName = "ProductApi";
                options.SupportedTokens = SupportedTokens.Both;
                options.ApiSecret = "productApi secret";
                options.RequireHttpsMetadata = true;
            });

            services.AddControllers();
            services.AddOcelot()
                .AddConsul()
                .AddCacheManager(x =>
                {
                    x.WithDictionaryHandle();
                })
                .AddPolly();//添加consul支持;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/order/swagger/v1/swagger.json", "Order API V1");
                c.SwaggerEndpoint("/product/swagger/v1/swagger.json", "Product API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
            IdentityModelEventSource.ShowPII = true;
            //设置Ocelot中间件
            app.UseOcelot().Wait();
        }
    }
}
