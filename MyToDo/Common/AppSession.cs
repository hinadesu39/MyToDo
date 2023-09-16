using MyToDo.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common
{
    /// <summary>
    /// 区域间简单消息通信
    /// </summary>
    public static class AppSession
    {
        public static UserDto UserDto { set; get; }
        public static string UserAvatar { set; get; }

        public static bool IsNewOpen { set; get; } = true;
        public static bool IsFirstLogin { set; get; } = true;
    }
}
