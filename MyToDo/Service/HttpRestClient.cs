using Microsoft.Win32;
using MyToDoApi.Sevice;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;
        public HttpRestClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
            client = new RestClient(apiUrl);
        }

        public async Task<ApiResponse> ExecuteAsync(BaseRequest baseRequest)
        {
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                string body = JsonConvert.SerializeObject(baseRequest.Parameter);
                request.AddStringBody(body, ContentType.Json);
            }
            RestResponse response = await client.ExecuteAsync(request);
           return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            
        }

        public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url,baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                string body = JsonConvert.SerializeObject(baseRequest.Parameter);
                request.AddStringBody(body,ContentType.Json);                 
            }
            RestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);

        }

    }
}
