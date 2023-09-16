namespace MyToDo.Extensions
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class QueryParameter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; } 

    }
}
