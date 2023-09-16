using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Dtos;
using MyToDo.Extensions;
using MyToDo.Service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyToDo.ViewModels.Dialogs
{
    public class UserCenterViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public DelegateCommand ChangeAvatarCommand { set; get; }

        private string userAvatar;

        public string UserAvatar
        {
            get { return userAvatar; }
            set { userAvatar = value; RaisePropertyChanged(); }
        }

        private UserDto userDto;

        public UserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
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
        private IDialogService _dialogService;
        private readonly ILoginService loginService;
        private readonly IEventAggregator aggregator;

        public UserCenterViewModel(IDialogService dialogService, ILoginService loginService, IEventAggregator aggregator)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            ChangeAvatarCommand = new DelegateCommand(ChangeAvatar);
            _dialogService = dialogService;
            this.loginService = loginService;
            this.aggregator = aggregator;
        }


        /// <summary>
        /// 更改用户头像
        /// </summary>
        private async void ChangeAvatar()
        {
            // 创建一个 OpenFileDialog 的实例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置初始目录为 C:\ 
            openFileDialog.InitialDirectory = "C:\\";
            // 设置过滤器为图片文件
            openFileDialog.Filter = "Image files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|All files (*.*)|*.*";
            // 设置标题为 Select an image file
            openFileDialog.Title = "Select an image file";
            // 显示对话框，并判断返回值
            openFileDialog.ShowDialog();
            // 获取用户选择的文件路径
            string filePath = openFileDialog.FileName;
            if (filePath == string.Empty) return;

            var newPath = await loginService.UploadAsync(filePath);
            if (newPath == null) return;
            newPath = Path.GetFileName(newPath);

            var res = await loginService.DownloadAsync(newPath);
            if (res == null) return;
            UserAvatar = res;
            UserDto.UserAvatar = Path.GetFileName(UserAvatar);
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private async void Save()
        {
            if (Password != null || Passwords != null)
            {
                if (Password != Passwords)
                {
                    aggregator.SendMessage("两次输入密码不一致！请重新输入");
                    return;
                }
                UserDto.Password = Password;

            }

            UserDto user = new UserDto()
            {
                Account = UserDto.Account,
                UserName = UserDto.UserName,
                UserAvatar = Path.GetFileName(UserDto.UserAvatar),
                Password = UserDto.Password.GetMD5(),
                Id = UserDto.Id,
                CreateDateTiem = UserDto.CreateDateTiem,
                UpdataDateTime = UserDto.UpdataDateTime
            };

            var res = await loginService.UpdateAsync(user);
            if (res.Status)
            {
                aggregator.SendMessage("保存成功");
                AppSession.UserDto = user;
                AppSession.UserAvatar = UserAvatar;
            }
            else
            {
                aggregator.SendMessage("保存失败，请稍后重试！");
            }


        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                UserDto = parameters.GetValue<UserDto>("Value");
            }
            else
            {
                UserDto = new UserDto();
            }
            UserAvatar = AppSession.UserAvatar;
        }
    }
}
