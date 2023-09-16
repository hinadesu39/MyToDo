namespace MyToDo.Extensions
{
    public class ToDoQueryParameter: QueryParameter
    {
        /// <summary>
        /// 是否按条件查询
        /// </summary>
        public int? Status { get; set; }
    }
}
