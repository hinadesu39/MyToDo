using MyToDo.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class MemoService:BaseService<MemoDto>, IMemoService
    {
        private readonly HttpRestClient client;
    public MemoService(HttpRestClient client) : base(client, "/Memo")
    {
        this.client = client;
    }
}
}
