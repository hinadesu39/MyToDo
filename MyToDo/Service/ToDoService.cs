using DryIoc;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDoApi.Sevice;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class ToDoService : BaseService<ToDoDto>, IToDoService
    {
        private readonly HttpRestClient client;
        public ToDoService(HttpRestClient client) : base(client, "/ToDo")
        {
            this.client=client;
        }

        public async Task<ApiResponse<List<ToDoDto>>> GetAllFilterAsync(ToDoQueryParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/ToDo/GetAll?PageIndex={parameter.PageIndex}&PageSize={parameter.PageSize}&Search={parameter.Search}&Status={parameter.Status}";
            var res = await client.ExecuteAsync<List<ToDoDto>>(request);
            return res;
        }

        public async Task<ApiResponse<SummaryDto>> Summary()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/ToDo/Summary";
            var res = await client.ExecuteAsync<SummaryDto>(request);
            return res;
        }
    }
}
