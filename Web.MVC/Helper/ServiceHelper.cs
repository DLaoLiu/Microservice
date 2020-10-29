using Consul;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.MVC.Helper
{
    public class ServiceHelper : IServiceHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ConsulClient _consulClient;
        private ConcurrentBag<string> _orderServiceUrls;
        private ConcurrentBag<string> _productServiceUrls;

        public ServiceHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _consulClient = new ConsulClient(c =>
            {
                //consul地址
                c.Address = new Uri(_configuration["ConsulSetting:ConsulAddress"]);
            });
        }
        public async Task<string> GetOrder(string accessToken)
        {
            #region 请求单个服务
            //string serviceUrl = "http://localhost:8790";//订单服务的地址，可以放在配置文件或者数据库等等...
            //var Client = new RestClient(serviceUrl);
            //var request = new RestRequest("/Order", Method.GET);
            //var response = await Client.ExecuteAsync(request);
            #endregion

            #region 请求多个服务
            //var consulClient = new ConsulClient(c => {
            //    c.Address = new Uri(_configuration["ConsulSetting:ConsulAddress"]);
            //});
            //var services = consulClient.Health.Service("OrderService", null, true, null).Result.Response;//健康的服务
            //string[] serviceUrls = services.Select(p => $"http://{p.Service.Address + ":" + p.Service.Port}").ToArray();//订单服务地址列表
            //if (!serviceUrls.Any())
            //{
            //    return await Task.FromResult("【订单服务】服务列表为空");
            //}
            ////每次随机访问一个服务实例
            //var Client = new RestClient(serviceUrls[new Random().Next(0, serviceUrls.Length)]);
            //var request = new RestRequest("/Order", Method.GET);
            //var response = await Client.ExecuteAsync(request);
            //return response.Content;
            #endregion

            #region 第三章
            if (_orderServiceUrls.IsEmpty)
                return await Task.FromResult("【订单服务】正在初始化服务列表...");

            //每次随机访问一个服务实例
            var Client = new RestClient(_orderServiceUrls.ElementAt(new Random().Next(0, _orderServiceUrls.Count())));
            var request = new RestRequest("/Order", Method.GET);

            var response = await Client.ExecuteAsync(request);
            return response.Content;

            #endregion
        }

        public async Task<string> GetProduct(string accessToken)
        {
            #region 请求单个服务
            //string serviceUrl = "http://localhost:8789";//产品服务的地址，可以放在配置文件或者数据库等等...
            //var Client = new RestClient(serviceUrl);
            //var request = new RestRequest("/Products", Method.GET);
            //var response = await Client.ExecuteAsync(request);
            #endregion


            #region 请求多个服务
            //var consulClient = new ConsulClient(c =>
            //{
            //    //consul地址
            //    c.Address = new Uri(_configuration["ConsulSetting:ConsulAddress"]);
            //});
            //var services = consulClient.Health.Service("ProductService", null, true, null).Result.Response;//健康的服务

            //string[] serviceUrls = services.Select(p => $"http://{p.Service.Address + ":" + p.Service.Port}").ToArray();//产品服务地址列表

            //if (!serviceUrls.Any())
            //{
            //    return await Task.FromResult("【订单服务】服务列表为空");
            //}
            ////每次随机访问一个服务实例
            //var Client = new RestClient(serviceUrls[new Random().Next(0, serviceUrls.Length)]);
            //var request = new RestRequest("/Products", Method.GET);
            //var response = await Client.ExecuteAsync(request);
            //return response.Content;
            #endregion


            #region 第三章
            if (_productServiceUrls == null)
                return await Task.FromResult("【产品服务】正在初始化服务列表...");

            //每次随机访问一个服务实例
            var Client = new RestClient(_productServiceUrls.ElementAt(new Random().Next(0, _productServiceUrls.Count())));
            var request = new RestRequest("/Products", Method.GET);

            var response = await Client.ExecuteAsync(request);
            return response.Content;
            #endregion

        }

        public void GetServices()
        {
            var serviceNames = new string[] { "OrderService", "ProductService" };
            Array.ForEach(serviceNames, p =>
            {
                Task.Run(() =>
                {
                    //WaitTime默认为5分钟
                    var queryOptions = new QueryOptions { WaitTime = TimeSpan.FromMinutes(10) };
                    while (true)
                    {
                        GetServices(queryOptions, p);
                    }
                });
            });
        }


        private void GetServices(QueryOptions queryOptions, string serviceName)
        {
            var res = _consulClient.Health.Service(serviceName, null, true, queryOptions).Result;

            //控制台打印一下获取服务列表的响应时间等信息
            Console.WriteLine($"{DateTime.Now}获取{serviceName}：queryOptions.WaitIndex：{queryOptions.WaitIndex}  LastIndex：{res.LastIndex}");

            //版本号不一致 说明服务列表发生了变化
            if (queryOptions.WaitIndex != res.LastIndex)
            {
                queryOptions.WaitIndex = res.LastIndex;

                //服务地址列表
                var serviceUrls = res.Response.Select(p => $"http://{p.Service.Address + ":" + p.Service.Port}").ToArray();

                if (serviceName == "OrderService")
                    _orderServiceUrls = new ConcurrentBag<string>(serviceUrls);
                else if (serviceName == "ProductService")
                    _productServiceUrls = new ConcurrentBag<string>(serviceUrls);
            }
        }
    }
}
