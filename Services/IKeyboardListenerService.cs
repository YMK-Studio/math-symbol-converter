using MathSymbolConverter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Services
{
    public interface IKeyboardListenerService
    {
        void StartKeyboardListener(Action<object, RawKeyEventArgs> keyDownEventHandler, Action<object, RawKeyEventArgs> keyUpEventHandler);
        void StopKeyboardListener();
        bool IsKeyboardListener();
    }
}
