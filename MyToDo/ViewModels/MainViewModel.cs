using FastExpressionCompiler.LightExpression;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Dtos;
using MyToDo.Extensions;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyToDo.ViewModels
{
    public class MainViewModel : NavigationViewModel, IConfigureService
    {
        private UserDto userDto;

        public UserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }

        private string userAvatar;

        public string UserAvatar
        {
            get { return userAvatar; }
            set { userAvatar = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }

        private readonly IRegionManager RegionManager;
        private readonly IContainerProvider container;
        private IRegionNavigationJournal journal;
        private readonly IDialogHostService dialog;
        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }
        public DelegateCommand UserCenterCommand { get; set; }
        public DelegateCommand LoginOutCommand { get; set; }



        public MainViewModel(IRegionManager regionManager, IContainerProvider Container, IDialogHostService dialog) : base(Container)
        {
            UserDto = new UserDto();
            MenuBars = new ObservableCollection<MenuBar>();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            RegionManager = regionManager;
            container = Container;
            GoBackCommand = new DelegateCommand(goback);
            UserCenterCommand = new DelegateCommand(goUserCenterAsync);
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                {
                    journal.GoForward();
                }
            });
            LoginOutCommand = new DelegateCommand(() =>
            {
                App.LoginOut(Container);
            });
            this.dialog = dialog;

            //主页中模块点击后界面跳转，但主界面左侧栏选中项也要更新
            eventAggregator.RegisterMessage(arg =>
            {
                if(arg.Message == "ToDoView")
                {
                    SelectedIndex = 1;
                }
                else
                {
                    SelectedIndex = 2;
                }
            }, "Navigate");

        }

        private async void goUserCenterAsync()
        {
            DialogParameters parameter = new DialogParameters();
            if (UserDto != null)
            {
                parameter.Add("Value", UserDto);
            }
            var res = await dialog.ShowDialog("UserCenterView", parameter);
            Configure();
        }

        private void goback()
        {
            if (journal != null && journal.CanGoBack)
            {
                journal.GoBack();
            }
        }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.NameSpace))
                return;
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, Callback =>
            {
                journal = Callback.Context.NavigationService.Journal;
            });


        }
        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlusOutline", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Configure()
        {
            if (MenuBars.Count == 0)
            {
                CreateMenuBar();
            }
            RegionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
            UserDto = AppSession.UserDto;
            UserAvatar = AppSession.UserAvatar;
        }
    }
}
