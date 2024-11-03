using MathSymbolConverter.Models;
using MathSymbolConverter.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MathSymbolConverter.Utilities;
using System.Windows.Input;
using System.Windows.Threading;

namespace MathSymbolConverter.ViewModels
{
    public class SettingsDialogViewModel : BindableBase, IDialogAware
    {
        #region Properties
        private string _title = "Settings";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        #endregion

        #region Notifying Properties
        private List<Symbol> _symbols;
        public List<Symbol> Symbols
        {
            get => _symbols;
            set => SetProperty(ref _symbols, value);
        }
        private List<Config> _configs;
        public List<Config> Configs
        {
            get => _configs;
            set => SetProperty(ref _configs, value);
        }
        private bool _canPopupPreview;
        public bool CanPopupPreview
        {
            get => _canPopupPreview;
            set => SetProperty(ref _canPopupPreview, value);
        }
        #endregion

        #region Fields & Ctor
        private readonly Action _saveSettingsAction;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IDatabaseService _databaseService;
        private readonly IKeyboardListenerService _keyboardListenerService;
        private readonly ISymbolManagerService _symbolManagerService;
        public SettingsDialogViewModel(IMessageBoxService messageBoxService, IDatabaseService databaseService, 
            IKeyboardListenerService keyboardListenerService, ISymbolManagerService symbolManagerService)
        {
            _saveSettingsAction = () => SaveSettings();
            _messageBoxService = messageBoxService;
            _databaseService = databaseService;
            _keyboardListenerService = keyboardListenerService;
            _symbolManagerService = symbolManagerService;

            CanPopupPreview = true;

            Symbols = _databaseService.GetOriginalSymbolsValue();
            Configs = _databaseService.GetOriginalConfigsValue();
        }
        #endregion

        private DelegateCommand _openPopupInPreviewCommand;
        public DelegateCommand OpenPopupInPreviewCommand => _openPopupInPreviewCommand ??= new DelegateCommand(OpenPopupInPreview);
        private void OpenPopupInPreview()
        {
            if(_keyboardListenerService.IsKeyboardListener())
            {
                _messageBoxService.Show("Please stop the macro before previewing the popup image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CanPopupPreview = false;
            _symbolManagerService.OpenPreviewWindow(_configs);
            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();

            void TimerTick(object sender, EventArgs e)
            {
                DispatcherTimer timer = (DispatcherTimer)sender;
                timer.Stop();
                timer.Tick -= TimerTick;
                _symbolManagerService.ClosePreviewWindow();
                CanPopupPreview = true;
            }
        }

        private DelegateCommand _resetSettingsCommand;
        public DelegateCommand ResetSettingsCommand => _resetSettingsCommand ??= new DelegateCommand(ResetSettings);
        private void ResetSettings()
        {
            if (_keyboardListenerService.IsKeyboardListener())
            {
                _messageBoxService.Show("Please stop the macro before resetting all settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (_messageBoxService.Show("Would you like to reset all settings?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.Reset();
                if(_databaseService.InitializeOriginalSymbols() && _databaseService.InitializeOriginalConfigs())
                {
                    Symbols = _databaseService.GetOriginalSymbolsValue();
                    Configs = _databaseService.GetOriginalConfigsValue();

                    _messageBoxService.Show("Reset successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private DelegateCommand _saveSettingsCommand;
        public DelegateCommand SaveSettingsCommand => _saveSettingsCommand ??= new DelegateCommand(_saveSettingsAction);
        private bool SaveSettings()
        {
            if (_keyboardListenerService.IsKeyboardListener())
            {
                _messageBoxService.Show("Please stop the macro before saving aliases.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            List<string> aliassTemp = new();
            foreach (Symbol gre in Symbols)
            {
                if (string.IsNullOrEmpty(gre.Alias))
                {
                    _messageBoxService.Show("One or more rules don't have an alias.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else if (!gre.Alias.All(c => char.IsLower(c)))
                {
                    _messageBoxService.Show("An alias can only contain lowercase letters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                {
                    aliassTemp.Add(gre.Alias);
                }
            }
            if (aliassTemp.Count != aliassTemp.Distinct().Count())
            {
                _messageBoxService.Show("Some rules share the same alias.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _databaseService.SetOriginalSymbolsTo(Symbols);
            _databaseService.SetOriginalConfigsTo(Configs);
            _databaseService.SaveOriginalSymbols();
            _databaseService.SaveOriginalConfigs();

            _messageBoxService.Show("Saved successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        #region IDialogAware Implementation

#pragma warning disable 67
        public event Action<IDialogResult> RequestClose;
        #pragma warning restore 67

        public bool CanCloseDialog()
        {
            Keyboard.ClearFocus();
            if (!(_databaseService.CompareOriginalSymbolsAliasTo(Symbols) && 
                _databaseService.CompareOriginalConfigsValueTo(Configs)))
            {
                MessageBoxResult result = _messageBoxService.Show("There are unsaved changes. Would you like to save?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    return SaveSettings();
                }
                else if (result == MessageBoxResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Keyboard.ClearFocus();
                return true;
            }
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        #endregion
    }
}
