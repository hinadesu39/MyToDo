using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Events
{
    /// <summary>
    /// 加载等待圈圈展示事件
    /// </summary>
    public class UpdateModel 
    {
        public bool IsOpen { set; get; }
    }

    public class UpdateLoadingEvent:PubSubEvent<UpdateModel>
    {
    }
}
