using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDoApi.Sevice;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class MemoViewModel : NavigationViewModel
    {


        private ObservableCollection<MemoDto> memoDtos;
        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
        }

        private string search;

        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }

        private MemoDto currentDto;

        public MemoDto  CurrentDto
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
        private readonly IMemoService MemoService;

        private readonly IDialogHostService dialogHostService;
        public DelegateCommand<string> ExecuteCommand { get; set; }

        public DelegateCommand<MemoDto> DeleteCommand { get; private set; }

        public DelegateCommand<MemoDto> SelectCommand { get; private set; }

        public MemoViewModel(IMemoService service, IContainerProvider containerProvider) : base(containerProvider)
        {

            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectCommand = new DelegateCommand<MemoDto>(Select);
            DeleteCommand = new DelegateCommand<MemoDto>(Delete);
            MemoService = service;
            dialogHostService = containerProvider.Resolve<DialogHostService>(); 
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

        private async void Delete(MemoDto memoDto)
        {
            var dialogRes = await dialogHostService.Question("温馨提示", $"你确定删除待办事项：{memoDto.Title}?");
            if (dialogRes.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
            UpdateLoading(true);
            var res = await MemoService.DeleteAsync(memoDto.Id);
            if (res.Status)
            {
                MemoDtos.Remove(memoDto);
            }
            UpdateLoading(false);
        }

        /// <summary>
        /// 保存或修改已存在项
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
                var todo = await MemoService.UpdateAsync(CurrentDto);
                if (todo.Status)
                {
                    var res = MemoDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
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
                var addRes = await MemoService.AddAsync(currentDto);
                if (addRes.Status)
                {
                    MemoDtos.Add(addRes.Result);

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
            CurrentDto = new Common.Models.MemoDto();
            IsDrawerOpen = true;

        }
        /// <summary>
        /// 展示选中内容
        /// </summary>
        /// <param name="obj"></param>
        private async void Select(MemoDto obj)
        {
            UpdateLoading(true);
            var todoResult = await MemoService.GetFirstOfDefaultAsync(obj.Id);
            CurrentDto = (MemoDto)todoResult.Result;
            IsDrawerOpen = true;
            UpdateLoading(false);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            

            UpdateLoading(true);
            MemoDtos = new ObservableCollection<Common.Models.MemoDto>();
            var toDoResult = await MemoService.GetAllAsync(new QueryParameter()
            {
                PageIndex = 1,
                PageSize = 100,
                Search = Search,               
            });

            MemoDtos.Clear();
            if (toDoResult != null)
            {
                foreach (var item in toDoResult.Result)
                {
                    MemoDtos.Add(item);
                }
            }
            UpdateLoading(false);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            GetDataAsync();
        }
    }
}
