using MathSymbolConverter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Services
{
    public interface IWindowService
    {
        void Close();
        void SetMainWindowToNormalState();
    }
}
