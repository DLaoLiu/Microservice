﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.MVC.Helper
{
    public class GatewayServiceHelper : IServiceHelper
    {
        public async Task<string> GetOrder(string accessToken)
        {
            var Client = new RestClient("http://localhost:9070");
            var request = new RestRequest("/Order", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return response.StatusCode + " " + response.Content;
            }
            return response.Content;
        }

        public async Task<string> GetProduct(string accessToken)
        {
            var Client = new RestClient("http://localhost:9070");
            var request = new RestRequest("/Products", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return response.StatusCode + " " + response.Content;
            }
            return response.Content;
        }

        public void GetServices()
        {
        }
    }
}
