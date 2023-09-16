﻿using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyToDo.ViewModels
{
    public class SkinViewModel : BindableBase, INavigationAware
    {
        //打开当前应用程序的配置文件
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public DelegateCommand<object> ChangeHueCommand { set; get; }
        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? Theme.Dark : Theme.Light));

                    //修改配置内容
                    config.AppSettings.Settings["IsDarkTheme"].Value = value.ToString();

                    //保存更改
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }
        public SkinViewModel()
        {
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);


        }

        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private void ChangeHue(object? obj)
        {
            //主题色号
            var hue = (Color)obj!;
            ITheme theme = paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(hue.Lighten());
            theme.PrimaryMid = new ColorPair(hue);
            theme.PrimaryDark = new ColorPair(hue.Darken());

            paletteHelper.SetTheme(theme);

            config.AppSettings.Settings["color"].Value = obj.ToString();

            //保存更改
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
           IsDarkTheme = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsDarkTheme"));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
