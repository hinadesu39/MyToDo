using MyToDo.Dtos;
using MyToDoApi.Sevice;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class LoginService : ILoginService
    {
        private readonly HttpRestClient client;
        public LoginService(HttpRestClient client)
        {
            this.client = client;
        }
        public async Task<ApiResponse<UserDto>> LoginAsync(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/Login/Login";
            request.Parameter = user;
            var res = await client.ExecuteAsync<UserDto>(request);
            return res;
        }

        public async Task<ApiResponse> RegisterAsync(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/Login/Register";
            request.Parameter = user;
            var res = await client.ExecuteAsync(request);
            return res;
        }

        public async Task<ApiResponse> UpdateAsync(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/Login/Update";
            request.Parameter = user;
            var res = await client.ExecuteAsync(request);
            return res;
        }

        public async Task<string?> UploadAsync(string Path)
        {

            var client = new RestClient();
            var request = new RestRequest("http://localhost:5058/api/UpLoader/Upload", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddFile("file", Path);
            RestResponse response = await client.ExecuteAsync(request);
            return response.Content.Replace("\"","");
        }


        public async Task<string> DownloadAsync(string key)
        {
            var client = new RestClient();
            var request = new RestRequest($"http://localhost:5058/Images/{key}", Method.Get);
            request.AlwaysMultipartFormData = true;
            var data = client.DownloadData(request);

            if (key.StartsWith("/"))
            {
                throw new ArgumentException("key should not start with /", nameof(key));

            }          
            
            // Get the solution directory
            string solutionDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            // Get the image file path
            string imagePath = Path.Combine(solutionDir, "Images", key);

            //给文件安排一个位置

            if (File.Exists(imagePath))
            {
                return imagePath;
            }
            //写入
            File.WriteAllBytes(imagePath, data);

            return imagePath;
        }


    }
}
