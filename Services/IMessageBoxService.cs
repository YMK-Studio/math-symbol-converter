using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathSymbolConverter.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageText);
        MessageBoxResult Show(string messageText, string caption);
        MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton);
        MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton, MessageBoxImage icon);
    }
}
