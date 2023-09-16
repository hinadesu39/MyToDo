using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDoApi.Sevice;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public interface IToDoService:IBaseService<ToDoDto>
    {
        Task<ApiResponse<List<ToDoDto>>> GetAllFilterAsync(ToDoQueryParameter parameter);
        Task<ApiResponse<SummaryDto>> Summary();
    }
}
