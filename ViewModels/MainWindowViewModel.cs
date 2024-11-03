using MathSymbolConverter.Models;
using MathSymbolConverter.Services;
using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace MathSymbolConverter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _title = "Math Symbol Converter";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _tooltipText;
        public string TooltipText
        {
            get => _tooltipText;
            set => SetProperty(ref _tooltipText, value);
        }

        private bool _showInTaskbar = true;
        public bool ShowInTaskbar
        {
            get => _showInTaskbar;
            set => SetProperty(ref _showInTaskbar, value);
        }

        private WindowState _windowState = WindowState.Normal;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                ShowInTaskbar = true;
                SetProperty(ref _windowState, value);
                ShowInTaskbar = value != WindowState.Minimized;
            }
        }
        #endregion

        #region Notifying Properties
        private bool _isHookingEnabled;
        public bool IsHookingEnabled
        {
            get => _isHookingEnabled;
            set => SetProperty(ref _isHookingEnabled, value);
        }
        #endregion

        #region Fields & Ctor
        private readonly IDialogService _dialogService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IWindowService _mainWindowService;
        private readonly IDatabaseService _databaseService;
        private readonly ISymbolManagerService _symbolManagerService;
        public MainWindowViewModel(IDialogService dialogService, IMessageBoxService messageBoxService, 
            IWindowService mainWindowService, IDatabaseService databaseService, ISymbolManagerService symbolManagerService)
        {
            _dialogService = dialogService;
            _messageBoxService = messageBoxService;
            _mainWindowService = mainWindowService;
            _databaseService = databaseService;
            _symbolManagerService = symbolManagerService;

            IsHookingEnabled = true;
            _databaseService.InitializeOriginalSymbols();
            _databaseService.InitializeOriginalConfigs();
        }
        #endregion

        private DelegateCommand _showSettingsDialogCommand;
        public DelegateCommand ShowSettingsDialogCommand => _showSettingsDialogCommand ??= new DelegateCommand(ShowSettingsDialog);
        void ShowSettingsDialog()
        {
            _dialogService.ShowDialog("SettingsDialog", null, null);
        }

        private DelegateCommand _closeMainWindowCommand;
        public DelegateCommand CloseMainWindowCommand => _closeMainWindowCommand ??= new DelegateCommand(CloseMainWindow);
        void CloseMainWindow()
        {
            _mainWindowService.Close();
        }

        private DelegateCommand _setMainWindowToNormalStateCommand;
        public DelegateCommand SetMainWindowToNormalStateCommand => _setMainWindowToNormalStateCommand ??= new DelegateCommand(SetMainWindowToNormalState);
        void SetMainWindowToNormalState()
        {
            _mainWindowService.SetMainWindowToNormalState();
        }

        //private DelegateCommand _showInfoMessageBoxCommand;
        //public DelegateCommand ShowInfoMessageBoxCommand => _showInfoMessageBoxCommand ??= new DelegateCommand(ShowInfoMessageBox);
        //void ShowInfoMessageBox()
        //{
        //    _messageBoxService.Show(
        //        "This is a Windows-exclusive macro program that helps you quickly input Greek letters or other useful mathematical symbols by using aliases.\n\n" +
        //        "Current version: 1.0\n"
        //        , _title, MessageBoxButton.OK, MessageBoxImage.None);
        //}

        private DelegateCommand _linkToGithubCommand;
        public DelegateCommand LinkToGithubCommand => _linkToGithubCommand ??= new DelegateCommand(LinkToGithub);
        void LinkToGithub()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/DylanKim35/math-symbol-converter",
                UseShellExecute = true
            });
        }

        private DelegateCommand _startHookingCommand;
        public DelegateCommand StartHookingCommand => _startHookingCommand ??= new DelegateCommand(StartHooking);
        void StartHooking()
        {
            IsHookingEnabled = false;
            TooltipText = "Macro Running...";
            _symbolManagerService.OpenWindow(_databaseService.Configs);
        }

        private DelegateCommand _stopHookingCommand;
        public DelegateCommand StopHookingCommand => _stopHookingCommand ??= new DelegateCommand(StopHooking);
        void StopHooking()
        {
            _symbolManagerService.CloseWindow();
            IsHookingEnabled = true;
            TooltipText = "Macro Not Running";
        }
        

        
    }
}