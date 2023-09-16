using MyToDo.Extensions;
using MyToDoApi.Sevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        private readonly HttpRestClient client;
        private readonly string serviceName;

        public BaseService(HttpRestClient client,string ServiceName)
        {
            this.client = client;
            serviceName = ServiceName;
        }
        public async Task<ApiResponse<TEntity>> AddAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api{serviceName}/Add";
            request.Parameter = entity;
            var res = await client.ExecuteAsync<TEntity>(request);
            return res;
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Delete;
            request.Route = $"api{serviceName}/Delete?id={id}";
            var res = await client.ExecuteAsync(request);
            return res;
        }

        public async Task<ApiResponse<List<TEntity>>> GetAllAsync(QueryParameter queryParameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api{serviceName}/GetAll?PageIndex={queryParameter.PageIndex}&PageSize={queryParameter.PageSize}&Search={queryParameter.Search}";
            var res = await client.ExecuteAsync<List<TEntity>>(request);
            return res;
        }

        public async Task<ApiResponse<TEntity>> GetFirstOfDefaultAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api{serviceName}/Get?id={id}";
            var res = await client.ExecuteAsync<TEntity>(request);
            return res;
        }

        public async Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api{serviceName}/Update";
            request.Parameter = entity;
            var res = await client.ExecuteAsync<TEntity>(request);
            return res;
        }
    }
}
