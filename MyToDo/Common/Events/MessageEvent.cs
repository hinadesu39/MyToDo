using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Events
{
    /// <summary>
    /// 消息事件
    /// </summary>
    public class MessageModel
    {
        public string Filter { set; get; }
        public string Message { set; get; }
    }

    public class MessageEvent : PubSubEvent<MessageModel>
    {

    }
}
