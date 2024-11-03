using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Services
{
    public class KeyboardListenerService : IKeyboardListenerService
    {
        private KeyboardListener _keyboardListener;

        public void StartKeyboardListener(Action<object, RawKeyEventArgs> keyDownEventHandler, Action<object, RawKeyEventArgs> keyUpEventHandler)
        {
            if(_keyboardListener == null )
            {
                _keyboardListener = new KeyboardListener();
                _keyboardListener.KeyDown += new RawKeyEventHandler(keyDownEventHandler);
                _keyboardListener.KeyUp += new RawKeyEventHandler(keyUpEventHandler);
            }
        }

        public void StopKeyboardListener()
        {
            if (_keyboardListener != null)
            {
                _keyboardListener.Dispose();
                _keyboardListener = null;
            }
        }

        public bool IsKeyboardListener()
        {
            return _keyboardListener != null;
        }
    }
}
