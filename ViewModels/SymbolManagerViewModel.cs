using MathSymbolConverter.Models;
using MathSymbolConverter.Services;
using MathSymbolConverter.Utilities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MathSymbolConverter.ViewModels
{
    public class SymbolManagerViewModel : BindableBase
    {
        #region Notifying Properties
        private bool _showsPopupWindow;
        public bool ShowsPopupWindow
        {
            get => _showsPopupWindow;
            set => SetProperty(ref _showsPopupWindow, value);
        }
        private string _popupName;
        public string PopupName
        {
            get => _popupName;
            set => SetProperty(ref _popupName, value);
        }
        private double _popupOpacity;
        public double PopupOpacity
        {
            get => _popupOpacity;
            set => SetProperty(ref _popupOpacity, value);
        }
        private double _popupSize;
        public double PopupSize
        {
            get => _popupSize;
            set => SetProperty(ref _popupSize, value);
        }
        private double _popupWidth;
        public double PopupWidth
        {
            get => _popupWidth;
            set => SetProperty(ref _popupWidth, value);
        }
        private Point _popupLocation;
        public Point PopupLocation
        {
            get => _popupLocation;
            set => SetProperty(ref _popupLocation, value);
        }
        private bool _isPopupColorBlack;
        public bool IsPopupColorBlack
        {
            get => _isPopupColorBlack;
            set
            {
                PopupColor = value ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
                SetProperty(ref _isPopupColorBlack, value);
            }
        }
        private SolidColorBrush _popupColor;
        public SolidColorBrush PopupColor
        {
            get => _popupColor;
            set => SetProperty(ref _popupColor, value);
        }
        #endregion

        #region Fields & Ctor
        readonly IKeyboardListenerService _keyboardListenerService;
        readonly IDatabaseService _databaseService;
        readonly ISymbolManagerService _symbolManagerService;
        readonly int _aliasMaxLength;
        readonly double _defaultPopupOpacity;

        readonly StringBuilder _textInput = new();
        int _aliasLengthOfPrint = 0;
        int _replicationCount = 0;
        readonly bool _isPopup = false;
        Symbol _symbol;

        public SymbolManagerViewModel(IKeyboardListenerService keyboardListenerService, IDatabaseService databaseService, ISymbolManagerService symbolManagerService)
        {
            _databaseService = databaseService;
            _symbolManagerService = symbolManagerService;

            PopupSize = double.Parse(_databaseService.GetConfigsValueByKey("PopupSize", _symbolManagerService.Configs));
            PopupWidth = PopupSize + 52.0;
            PopupOpacity = double.Parse(_databaseService.GetConfigsValueByKey("PopupOpacity", _symbolManagerService.Configs));
            PopupLocation = Int32.Parse(_databaseService.GetConfigsValueByKey("PopupLocation", _symbolManagerService.Configs)) switch
            {
                0 => new Point(0, 0),
                1 => new Point(0, SystemParameters.WorkArea.Height - PopupWidth),
                2 => new Point(SystemParameters.WorkArea.Width - PopupWidth, 0),
                3 => new Point(SystemParameters.WorkArea.Width - PopupWidth, SystemParameters.WorkArea.Height - PopupWidth),
                4 => new Point((SystemParameters.WorkArea.Width - PopupWidth) / 2, (SystemParameters.WorkArea.Height - PopupWidth) / 2),
                _ => new Point(0, 0)
            };
            IsPopupColorBlack = bool.Parse(_databaseService.GetConfigsValueByKey("IsPopupColorBlack", _symbolManagerService.Configs));

            if (_symbolManagerService.IsPreview)
            {
                PopupName = "α";
                ShowsPopupWindow = true;
                _symbolManagerService.IsTopmost(true);
            }
            else
            {
                PopupName = null;
                _isPopup = bool.Parse(_databaseService.GetConfigsValueByKey("IsPopup", _symbolManagerService.Configs));
                _defaultPopupOpacity = PopupOpacity;
                _keyboardListenerService = keyboardListenerService;
                _aliasMaxLength = _databaseService.GetMaxLengthOfAlias();
                _keyboardListenerService.StartKeyboardListener(KeyboardListener_KeyDown, KeyboardListener_KeyUp);
                _textInput.Append(' ');
            }
        }
        #endregion

        private DelegateCommand _closeSymbolManagerCommand;
        public DelegateCommand CloseSymbolManagerCommand => _closeSymbolManagerCommand ??= new DelegateCommand(CloseSymbolManager);
        void CloseSymbolManager()
        {
            _keyboardListenerService?.StopKeyboardListener();
        }

        void KeyboardListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            // Debug.WriteLine(args.VKCode);
            if (args.ASCII == 20 || args.ASCII == 21 || (args.ASCII >= 160 && args.ASCII <= 163)) return;

            if (PopupName != null)
            {
                if (args.ASCII == 9)    // TAB
                {
                    if (_isPopup)
                    {
                        PopupOpacity = 1.0;
                    }

                    if (!_textInput.ToString().Equals(" "))
                    {
                        _textInput.Clear();
                        _textInput.Append(' ');
                    }
                    return;
                }
                else if (args.ASCII >= 37 && args.ASCII <= 40 && _textInput.ToString().Equals(" "))
                {
                    return;
                }

                PopupName = null;
                if (_isPopup)
                {
                    _symbolManagerService.IsTopmost(false);
                    ShowsPopupWindow = false;
                    PopupOpacity = _defaultPopupOpacity;
                }
                _aliasLengthOfPrint = 0;
                _replicationCount = 0;
            }
            else if (args.ASCII == 9)
            {
                KeyboardWriter.TriggerDefaultTabKeyDownEvent();
            }

            if (args.ASCII == 220)     // backslash
            {
                _textInput.Clear();
            }
            else if ((_textInput.Length < _aliasMaxLength + 5) && !_textInput.ToString().Equals(" "))
            {
                if (0x41 <= args.ASCII && args.ASCII <= 0x5A)    // Capital letter
                {
                    _textInput.Append(args.ToString().ToLower());
                    AllocateSymbolInfoIfAliasExists(_textInput.ToString());
                    return;
                }
                else if (args.ASCII == 8 && _textInput.Length > 0)
                {
                    _textInput.Length--;
                    AllocateSymbolInfoIfAliasExists(_textInput.ToString());
                    return;
                }
                _textInput.Clear();
                _textInput.Append(' ');
            }

            void AllocateSymbolInfoIfAliasExists(string alias)
            {
                _symbol = _databaseService.GetOriginalSymbolByAlias(alias);
                if (_symbol != null)
                {
                    PopupName = _symbol.Name;
                    _aliasLengthOfPrint = _symbol.Alias.Length;

                    if (_isPopup)
                    {
                        _symbolManagerService.IsTopmost(true);
                        ShowsPopupWindow = true;
                    }
                }
            }
        }

        void KeyboardListener_KeyUp(object sender, RawKeyEventArgs args)
        {
            if (args.ASCII == 9)
            {
                if(PopupName != null)
                {
                    if(_isPopup)
                        PopupOpacity = _defaultPopupOpacity;

                    if (_replicationCount == 0)
                    {
                        KeyboardWriter.PrintText(PopupName, _aliasLengthOfPrint);
                    }
                    else
                    {
                        KeyboardWriter.PrintText(PopupName, 0);
                    }
                    _replicationCount++;
                }
                else
                {
                    KeyboardWriter.TriggerDefaultTabKeyUpEvent();
                }
            }
        }
    }
}
