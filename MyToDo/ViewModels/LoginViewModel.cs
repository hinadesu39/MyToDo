using DryIoc;
using MyToDo.Common;
using MyToDo.Dtos;
using MyToDo.Extensions;
using MyToDo.Service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input.StylusPlugIns;

namespace MyToDo.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {

        //打开当前应用程序的配置文件
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private readonly IEventAggregator aggregator;
        private readonly ILoginService LoginService;
        public string Title { set; get; } = "ToDo";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<string> ExecuteCommand { get; set; }

        private bool isRemember;
        public bool IsRemember
        {
            get { return isRemember; }
            set { isRemember = value; RaisePropertyChanged(); }
        }

        private bool isAutoLogin;
        public bool IsAutoLogin
        {
            get { return isAutoLogin; }
            set { isAutoLogin = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private string passwords;
        public string Passwords
        {
            get { return passwords; }
            set { passwords = value; RaisePropertyChanged(); }
        }

        public LoginViewModel(ILoginService loginService, IEventAggregator aggregator)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.LoginService = loginService;
            this.aggregator = aggregator;
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "登录":
                    Login();
                    break;
                case "忘记密码":
                    aggregator.SendMessage("请联系管理员","Login");
                    break;
                case "注册账号":
                    SelectedIndex = 1;
                    break;
                case "注册":
                    SelectedIndex = 1;
                    Register();
                    break;
                case "返回":
                    SelectedIndex = 0;
                    break;
            }
        }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Account))
            {
                return;
            }
            if(Password != Passwords)
            {
                aggregator.SendMessage("两次输入密码不一致", "Login");
                Password = "";
                Passwords = "";
                return;
            }
            UserDto userDto = new UserDto();
            userDto.UserName = Account;
            userDto.Password = Password.GetMD5();
            userDto.Account = Account;
            userDto.UserAvatar = "default.png";
            var RegisterRes = await LoginService.RegisterAsync(userDto);
            if (RegisterRes.Status)
            {
                SelectedIndex = 0;
                aggregator.SendMessage("注册成功", "Login");
            }
            else
            {
                aggregator.SendMessage((string)RegisterRes.Result, "Login");
            }
            

        }

        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Account))
            {
                return;
            }
            UserDto userDto= new UserDto();
            userDto.Password = Password.GetMD5();    
            userDto.Account = Account;
            userDto.UserName = Account;
            var loginRes =await LoginService.LoginAsync(userDto);
            if (loginRes.Status) 
            {
                AppSession.UserDto = loginRes.Result;
                var path = await LoginService.DownloadAsync(loginRes.Result.UserAvatar);
                AppSession.UserAvatar = path;
                AppSession.UserDto.UserAvatar =Path.GetFileName(path);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));

                //记住当前用户信息
                config.AppSettings.Settings["IsRemember"].Value = IsRemember.ToString();
                config.AppSettings.Settings["IsAutoLogin"].Value = IsAutoLogin.ToString();
                if (IsRemember)
                {
                    config.AppSettings.Settings["Account"].Value = Account.ToString();
                    config.AppSettings.Settings["Password"].Value = Password.ToString();
                }


                aggregator.SendMessage("登录成功");
            }
            else
            {
                aggregator.SendMessage(loginRes.Message+" fail!!!","Login");
            }
            
        }

        private void LoginOut()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.No));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        public void OnDialogClosed()
        {
            config.AppSettings.Settings["IsRemember"].Value = IsRemember.ToString();
            config.AppSettings.Settings["IsAutoLogin"].Value = IsAutoLogin.ToString();
            if (IsRemember)
            {
                config.AppSettings.Settings["Account"].Value = Account.ToString();
                config.AppSettings.Settings["Password"].Value = Password.ToString();
            }
        }
            
        /// <summary>
        /// 初始化展示界面
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsRemember = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsRemember"));
            IsAutoLogin = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsAutoLogin"));
            if (IsRemember)
            {
                Account = ConfigurationManager.AppSettings.Get("Account");
                Password = ConfigurationManager.AppSettings.Get("Password");
            }
            if (IsAutoLogin&&AppSession.IsFirstLogin)
            {
                AppSession.IsFirstLogin = false;
                Login();
            }
        }
    }
}
