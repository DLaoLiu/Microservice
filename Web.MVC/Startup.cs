using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Web.MVC.Helper;

namespace Web.MVC
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "http://localhost:9070/auth";//ͨ�����ط��ʼ�Ȩ����
                //options.Authority = "http://localhost:9080";

                options.ClientId = "web client";
                options.ClientSecret = "web client secret";
                options.ResponseType = "code";


                options.RequireHttpsMetadata = false;

                options.SaveTokens = true;

                options.Scope.Add("orderApiScope");
                options.Scope.Add("productApiScope");

            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            //ע��IServiceHelper
            //services.AddSingleton<IServiceHelper,ServiceHelper>();
            //ע��IServiceHelper
            services.AddSingleton<IServiceHelper, GatewayServiceHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceHelper serviceHelper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            IdentityModelEventSource.ShowPII = true; // here
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            /*�����֤*/
            app.UseAuthentication();

            /*��Ȩ*/
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });
            //��������ʱ ��ȡ�����б�
            //serviceHelper.GetServices();


        }
    }
}
