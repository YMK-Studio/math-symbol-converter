using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathSymbolConverter.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string messageText)
        {
            return MessageBox.Show(messageText);
        }

        public MessageBoxResult Show(string messageText, string caption)
        {
            return MessageBox.Show(messageText, caption);
        }

        public MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton)
        {
            return MessageBox.Show(msg, caption, btnsMessageBoxButton);
        }

        public MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton, MessageBoxImage icon)
        {
            return MessageBox.Show(msg, caption, btnsMessageBoxButton, icon);
        }
    }
}
