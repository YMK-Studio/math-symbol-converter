using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathSymbolConverter.Services
{
    public class WindowService : IWindowService
    {
        public void Close()
        {
            Environment.Exit(0);
        }

        public void SetMainWindowToNormalState()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }


    }
}
