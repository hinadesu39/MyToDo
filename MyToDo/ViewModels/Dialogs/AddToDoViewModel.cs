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
    /// <summary>
    /// 原生版本继承IDialogAware
    /// </summary>
    public class AddToDoViewModel :BindableBase, IDialogHostAware
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
        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; RaisePropertyChanged(); }
        }
        private ToDoDto toDoDto;

        public ToDoDto ToDoDto
        {
            get { return toDoDto; }
            set { toDoDto = value; RaisePropertyChanged(); }
        }


        public AddToDoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                ToDoDto = parameters.GetValue<ToDoDto>("Value");
                Title = ToDoDto.Title;
                Content = ToDoDto.Content;
                Status = ToDoDto.Status;
            }
            else
            {
                ToDoDto = new ToDoDto();
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private void Save()
        {

            if (string.IsNullOrWhiteSpace(ToDoDto.Content) || string.IsNullOrWhiteSpace(ToDoDto.Title)) return;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                ToDoDto.Status = Status;
                ToDoDto.Content = Content;
                ToDoDto.Title = Title;
                DialogParameters parameters = new DialogParameters();
                parameters.Add("Value",ToDoDto);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }

        }
    }
}
