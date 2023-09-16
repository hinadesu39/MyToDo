using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    /// <summary>
    /// 汇总信息
    /// </summary>
    public class SummaryDto : BaseDto
    {
        private int sum;
        private int completedCount;
        private int memoCount;
        private string completedRate;

        private ObservableCollection<ToDoDto> toDoList;
        private ObservableCollection<MemoDto> memoList;

        public int Sum
        {
            set { sum = value; OnPropertyChanged(); }
            get { return sum; }
        }
        public int CompletedCount
        {
            set { completedCount = value; OnPropertyChanged(); }
            get { return completedCount; }
        }
        public int MemoCount
        {
            set { memoCount = value; OnPropertyChanged(); }
            get { return memoCount; }
        }
        public string CompletedRate
        {
            set { completedRate = value; OnPropertyChanged(); }
            get { return completedRate; }
        }


        public ObservableCollection<ToDoDto> ToDoList
        {
            set { toDoList = value; OnPropertyChanged(); }
            get { return toDoList; }
        }

        public ObservableCollection<MemoDto> MemoList
        {
            set { memoList = value; OnPropertyChanged(); }
            get { return memoList; }
        }
    }
}
