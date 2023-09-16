using DryIoc;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDoApi.Sevice;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class ToDoViewModel : NavigationViewModel
    {
        private int selectedIndex;


        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<ToDoDto> toDoDtos;
        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }

        private string search;

        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }

        private ToDoDto currentDto;

        public ToDoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        private bool isDrawerOpen;

        /// <summary>
        /// 右侧窗口是否展开
        /// </summary>
        public bool IsDrawerOpen
        {
            get { return isDrawerOpen; }
            set { isDrawerOpen = value; RaisePropertyChanged(); }
        }
        private readonly IToDoService ToDoService;

        private readonly IDialogHostService dialogHostService;
        public DelegateCommand<string> ExecuteCommand { get; set; }

        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }

        public DelegateCommand<ToDoDto> SelectCommand { get; private set; }

        public ToDoViewModel(IToDoService service, IContainerProvider containerProvider) : base(containerProvider)
        {

            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectCommand = new DelegateCommand<ToDoDto>(Select);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);
            ToDoService = service;
            this.dialogHostService = containerProvider.Resolve<IDialogHostService>();
        }


        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增":
                    Add();
                    break;
                case "查询":
                    Query();
                    break;
                case "保存":
                    Save();
                    break;

            }

        }

        private async void Delete(ToDoDto toDoDto)
        {
            var dialogRes = await dialogHostService.Question("温馨提示", $"你确定删除待办事项：{toDoDto.Title}?");
            if (dialogRes.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
            UpdateLoading(true);
            var res = await ToDoService.DeleteAsync(toDoDto.Id);
            if (res.Status)
            {
                ToDoDtos.Remove(toDoDto);
            }
            UpdateLoading(false);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) || string.IsNullOrWhiteSpace(CurrentDto.Content))
            {
                return;
            }
            UpdateLoading(true);

            if (CurrentDto.Id > 0)
            {
                var todo = await ToDoService.UpdateAsync(CurrentDto);
                if (todo.Status)
                {
                    var res = ToDoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                    if (res != null)
                    {
                        res.Title = CurrentDto.Title;
                        res.Content = CurrentDto.Content;
                        res.Status = CurrentDto.Status;
                    }
                }
            }
            else
            {
                var addRes = await ToDoService.AddAsync(currentDto);
                if (addRes.Status)
                {
                    ToDoDtos.Add(addRes.Result);


                }
            }
            Query();
            IsDrawerOpen = false;
            UpdateLoading(false);
        }

        /// <summary>
        /// 查询
        /// </summary>
        private void Query()
        {
            GetDataAsync();
        }

        /// <summary>
        /// 添加待办
        /// </summary>
        private void Add()
        {
            CurrentDto = new ToDoDto();
            IsDrawerOpen = true;

        }
        /// <summary>
        /// 展示选中内容
        /// </summary>
        /// <param name="obj"></param>
        private async void Select(ToDoDto obj)
        {
            UpdateLoading(true);
            var todoResult = await ToDoService.GetFirstOfDefaultAsync(obj.Id);
            CurrentDto = (ToDoDto)todoResult.Result;
            IsDrawerOpen = true;
            UpdateLoading(false);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            int? Status = selectedIndex == 0 ? null : SelectedIndex == 2 ? 1 : 0;

            UpdateLoading(true);
            ToDoDtos = new ObservableCollection<ToDoDto>();
            var toDoResult = await ToDoService.GetAllFilterAsync(new ToDoQueryParameter()
            {
                PageIndex = 1,
                PageSize = 100,
                Search = Search,
                Status = Status
            });

            ToDoDtos.Clear();
            if (toDoResult != null)
            {
                foreach (var item in toDoResult.Result)
                {
                    ToDoDtos.Add(item);
                }
            }
            UpdateLoading(false);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.ContainsKey("Value"))
            {
                SelectedIndex = navigationContext.Parameters.GetValue<int>("Value");
            }
            else
            {
                SelectedIndex = 0;
            }
            GetDataAsync();
        }
    }
}
