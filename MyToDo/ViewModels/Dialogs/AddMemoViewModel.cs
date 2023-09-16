using DryIoc;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels.Dialogs
{
    public class AddMemoViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }
        private MemoDto memeDto;

        public MemoDto MemoDto
        {
            get { return memeDto; }
            set { memeDto = value; RaisePropertyChanged(); }
        }

        public AddMemoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        /// <summary>
        /// 初始化显示内容
        /// </summary>
        /// <param name="parameters"></param>
        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                MemoDto = parameters.GetValue<MemoDto>("Value");
                Content = MemoDto.Content;
                Title = MemoDto.Title;
            }
            else
            {
                MemoDto = new MemoDto();
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(MemoDto.Content) || string.IsNullOrWhiteSpace(MemoDto.Title)) return;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                MemoDto.Content = Content;
                MemoDto.Title = Title;
                DialogParameters parameters = new DialogParameters();
                parameters.Add("Value", MemoDto);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }

        }
    }
}
