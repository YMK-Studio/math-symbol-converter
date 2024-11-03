using MathSymbolConverter.Models;
using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Services
{
    public interface ISymbolManagerService
    {
        SymbolManager SymbolManager { get; }
        List<Config> Configs { get; }
        bool IsPreview { get; }
        void CloseWindow();
        void ClosePreviewWindow();
        void OpenWindow(List<Config> configs);
        void OpenPreviewWindow(List<Config> configs);
        void IsTopmost(bool value);
    }
}
