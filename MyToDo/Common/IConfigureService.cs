using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common
{
    public interface IConfigureService
    {
        /// <summary>
        /// 配置界面初始化
        /// </summary>
        void Configure();
    }
}
