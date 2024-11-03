using MathSymbolConverter.Models;
using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MathSymbolConverter.Services
{
    public class SymbolManagerService : ISymbolManagerService
    {
        private SymbolManager _symbolManager;
        public SymbolManager SymbolManager => _symbolManager;

        private List<Config> _configs;
        public List<Config> Configs => _configs;

        private bool _isPreview;
        public bool IsPreview => _isPreview;

        public void CloseWindow()
        {
            if( _symbolManager != null )
            {
                _isPreview = false;
                _symbolManager.Close();
            }
            _symbolManager = null;
            _configs = null;
        }

        public void ClosePreviewWindow()
        {
            if (_symbolManager != null && _isPreview)
            {
                _isPreview = false;
                _symbolManager.Close();
                _symbolManager = null;
                _configs = null;
            }
        }

        public void OpenWindow(List<Config> configs)
        {
            CloseWindow();
            _configs = configs;
            _symbolManager = new();
            _symbolManager.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _symbolManager.Show();
        }

        public void OpenPreviewWindow(List<Config> configs)
        {
            if (_symbolManager == null)
            {
                _isPreview = true;
                _configs = configs;
                _symbolManager = new();
                _symbolManager.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                _symbolManager.Show();
            }
        }

        public void IsTopmost(bool value)
        {
            if (_symbolManager != null)
            {
                _symbolManager.Topmost = value;
            }
        }
    }
}
