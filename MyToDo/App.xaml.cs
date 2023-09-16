using DryIoc;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Service;
using MyToDo.ViewModels;
using MyToDo.ViewModels.Dialogs;
using MyToDo.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyToDo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        public static void LoginOut(IContainerProvider Container)
        {
            Current.MainWindow.Hide();           
             var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog("LoginView", callBack =>
            {
                if (callBack.Result == ButtonResult.OK)
                {
                    Current.MainWindow.Show();
                    var service = App.Current.MainWindow.DataContext as IConfigureService;
                    if (service != null)
                    {
                        service.Configure();
                    }
                }
                else
                {
                    Environment.Exit(0);
                    return;
                }

            });
        }

        protected override void OnInitialized()
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            var IsDarkTheme = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsDarkTheme"));
            theme.SetBaseTheme(IsDarkTheme ? Theme.Dark : Theme.Light);


            var color = (Color)ColorConverter.ConvertFromString(ConfigurationManager.AppSettings.Get("color"));

            theme.PrimaryLight = new ColorPair(color);
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color);

            paletteHelper.SetTheme(theme);
            var dialog = Container.Resolve<IDialogService>();
            dialog.ShowDialog("LoginView", callBack =>
            {
                if (callBack.Result == ButtonResult.OK)
                {
                    var service = App.Current.MainWindow.DataContext as IConfigureService;
                    if (service != null)
                    {
                        service.Configure();
                    }
                    base.OnInitialized();
                }
                else
                {
                    Environment.Exit(0);
                    return;
                }

            });

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer().Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost:5058/", serviceKey: "webUrl");


            containerRegistry.Register<IToDoService, ToDoService>();
            containerRegistry.Register<IMemoService, MemoService>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.Register<ILoginService, LoginService>();

            //containerRegistry.RegisterDialog<AddMemoView,AddMemoViewModel>();
            //containerRegistry.RegisterDialog<AddToDoView,AddToDoViewModel>();           
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

            containerRegistry.RegisterForNavigation<AddMemoView, AddMemoViewModel>();
            containerRegistry.RegisterForNavigation<AddToDoView, AddToDoViewModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.RegisterForNavigation<UserCenterView, UserCenterViewModel>();


            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>();
            containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>();
            containerRegistry.RegisterForNavigation<AboutView>();
        }
    }
}
