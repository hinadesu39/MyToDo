using MyToDo.Dtos;
using MyToDoApi.Sevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public interface ILoginService
    {
        Task<ApiResponse<UserDto>> LoginAsync(UserDto user);
        Task<ApiResponse> RegisterAsync(UserDto user);
        Task<ApiResponse> UpdateAsync(UserDto user);
        Task<string?> UploadAsync(string Path);

        Task<string> DownloadAsync(string key);
    }
}
