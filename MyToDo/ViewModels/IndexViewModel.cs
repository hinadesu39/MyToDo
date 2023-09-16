using Microsoft.VisualBasic;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        public DelegateCommand<TaskBar> NevigateCommand { get; set; }
        public DelegateCommand<ToDoDto> EditToDoCommand { get; set; }
        public DelegateCommand<MemoDto> EditMemoCommand { get; set; }
        public DelegateCommand<ToDoDto> ToDoCompletedCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<TaskBar> taskBars;
        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }



        private SummaryDto summaryDto;
        public SummaryDto SummaryDto
        {
            get { return summaryDto; }
            set { summaryDto = value; RaisePropertyChanged(); }
        }

        private readonly IDialogHostService dialog;
        private readonly IMemoService memoService;
        private readonly IToDoService toDoService;
        private readonly IRegionManager regionManager;

        public DelegateCommand<string> ExecuteCommand { get; set; }
        public IndexViewModel(IDialogHostService dialog, IContainerProvider provider) : base(provider)
        {
            UserName = AppSession.UserDto.UserName;
            TaskBars = new ObservableCollection<TaskBar>();
            CreateTaskbars();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            EditToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompletedCommand = new DelegateCommand<ToDoDto>(ToDoCompleted);
            NevigateCommand = new DelegateCommand<TaskBar>(Nevigate);
            this.dialog = dialog;
            memoService = provider.Resolve<IMemoService>();
            toDoService = provider.Resolve<IToDoService>();
            regionManager = provider.Resolve<IRegionManager>();
            Title = $"你好，{UserName} {DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
        }

        /// <summary>
        /// 导航命令
        /// </summary>
        /// <param name="obj"></param>
        private void Nevigate(TaskBar obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Target))
            {
                return;
            }
            NavigationParameters para = new NavigationParameters();

            if (obj.Title == "已完成")
            {
                para.Add("Value", 2);
            }
            eventAggregator.SendMessage(obj.Target.ToString(), "Navigate");
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.Target, para);
        }

        private async void ToDoCompleted(ToDoDto obj)
        {
            var res = await toDoService.UpdateAsync(obj);
            if (res.Status)
            {

                var ToUpdate = SummaryDto.ToDoList.FirstOrDefault(m => m.Id == obj.Id);
                if (ToUpdate != null)
                {
                    SummaryDto.ToDoList.Remove(ToUpdate);
                    SummaryDto.CompletedCount += 1;
                    SummaryDto.CompletedRate = (SummaryDto.CompletedCount / (double)SummaryDto.Sum).ToString("0%");
                    this.Refresh();
                }
                eventAggregator.SendMessage("已完成!");
            }
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办":
                    AddToDo(null);
                    break;
                case "新增备忘录":
                    AddMemo(null);
                    break;
            }
        }

        /// <summary>
        /// 打开添加页面，并根据返回的结果执行更新或者添加操作
        /// </summary>
        /// <param name="dto"></param>
        private async void AddMemo(MemoDto dto)
        {
            DialogParameters parameter = new DialogParameters();
            if (dto != null)
            {
                parameter.Add("Value", dto);
            }
            var res = await dialog.ShowDialog("AddMemoView", parameter);
            if (res.Result == ButtonResult.OK)
            {
                UpdateLoading(true);
                var memo = res.Parameters.GetValue<MemoDto>("Value");
                if (memo.Id > 0)
                {
                    var updateRes = await memoService.UpdateAsync(dto);
                    if (updateRes.Status)
                    {
                        var ToUpdate = SummaryDto.MemoList.FirstOrDefault(m => m.Id == dto.Id);
                        if (ToUpdate != null)
                        {
                            ToUpdate.Title = dto.Title;
                            ToUpdate.Content = dto.Content;

                        }
                    }
                }
                else
                {
                    var addRes = await memoService.AddAsync(memo);
                    if (addRes.Status)
                    {
                        SummaryDto.MemoList.Add(addRes.Result);

                    }
                }

                UpdateLoading(false);
            }
            this.Refresh();
        }

        private async void AddToDo(ToDoDto dto)
        {


            DialogParameters parameter = new DialogParameters();
            if (dto != null)
            {
                parameter.Add("Value", dto);
            }
            var res = await dialog.ShowDialog("AddToDoView", parameter);
            if (res.Result == ButtonResult.OK)
            {
                UpdateLoading(true);
                var todo = res.Parameters.GetValue<ToDoDto>("Value");

                if (todo.Id > 0)
                {
                    var updateRes = await toDoService.UpdateAsync(dto);
                    if (updateRes.Status)
                    {
                        var ToUpdate = SummaryDto.ToDoList.FirstOrDefault(m => m.Id == dto.Id);
                        if (ToUpdate != null)
                        {
                            ToUpdate.Title = dto.Title;
                            ToUpdate.Content = dto.Content;
                            ToUpdate.Status = dto.Status;
                        }

                    }
                }
                else
                {
                    var addRes = await toDoService.AddAsync(todo);
                    if (addRes.Status)
                    {
                        SummaryDto.ToDoList.Add(addRes.Result);
                        SummaryDto.Sum += 1;
                        SummaryDto.CompletedRate = (SummaryDto.CompletedCount / (double)SummaryDto.Sum).ToString("0%");

                    }
                }
                UpdateLoading(false);
            }
            this.Refresh();
        }

        /// <summary>
        /// 初始化显示模块
        /// </summary>
        void CreateTaskbars()
        {
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Color = "#FF0CA0FF", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Color = "#FF1ECA3A", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "完成率", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Color = "#FFFFA000", Target = "MemoView" });

        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var res = await toDoService.Summary();
            if (res.Status)
            {
                SummaryDto = res.Result;
                Refresh();
            }
            base.OnNavigatedTo(navigationContext);
        }

        /// <summary>
        /// 数据更新后刷新界面
        /// </summary>
        void Refresh()
        {
            TaskBars[0].Content = SummaryDto.Sum.ToString();
            TaskBars[1].Content = SummaryDto.CompletedCount.ToString();
            TaskBars[2].Content = SummaryDto.CompletedRate.ToString();
            TaskBars[3].Content = SummaryDto.MemoList.Count.ToString();
            UserName = AppSession.UserDto.UserName;
            Title = $"你好，{UserName} {DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
        }
    }
}
