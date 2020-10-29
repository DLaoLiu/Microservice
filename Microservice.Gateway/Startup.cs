using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
//using IdentityServer4.AccessTokenValidation;
//using Microservice.Gateway.Common.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
//using Ocelot.Provider.Consul;
//using Ocelot.Cache.CacheManager;
//using Ocelot.Provider.Polly;

namespace Microservice.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "Gateway API", Version = "v1", Description = "# gateway api..." });
            });

            services.AddControllers();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                //.AddIdentityServerAuthentication("orderService", options =>
                //{
                //    //macϵͳ��docker.for.mac.localhost
                //    //linuxϵͳ��docker.for.linux.localhost
                //    options.Authority = "http://docker.for.win.localhost:9080";//��Ȩ���ĵ�ַ
                //    options.ApiName = "orderApi";
                //    options.SupportedTokens = SupportedTokens.Both;
                //    options.ApiSecret = "orderApi secret";
                //    options.RequireHttpsMetadata = false;
                //})
                .AddIdentityServerAuthentication("ProductService", options =>
                {
                    //macϵͳ��docker.for.mac.localhost
                    //linuxϵͳ��docker.for.linux.localhost
                    options.Authority = "http://127.0.0.1:8500";//"http://docker.for.win.localhost:9080";//��Ȩ���ĵ�ַ
                    options.ApiName = "productApi";
                    options.SupportedTokens = SupportedTokens.Both;
                    options.ApiSecret = "productApi secret";
                    options.RequireHttpsMetadata = false;
                });

            //���ocelot����
            services.AddOcelot();
                ////���consul֧��
                //.AddConsul()
                ////��ӻ���
                //.AddCacheManager(x =>
                //{
                //    x.WithDictionaryHandle();
                //})
                ////���Polly
                //.AddPolly();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/Order/swagger/v1/swagger.json", "Order API V1");
                c.SwaggerEndpoint("/Products/swagger/v1/swagger.json", "Product API V1");
            });

            //����Ocelot�м��
            app.UseOcelot().Wait();
        }
    }
}
