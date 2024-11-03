using MathSymbolConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Services
{
    public interface IDatabaseService
    {
        List<Symbol> Symbols { get; }
        List<Config> Configs { get; }
        bool InitializeOriginalSymbols();
        bool InitializeOriginalConfigs();
        void SaveOriginalSymbols();
        void SaveOriginalConfigs();
        bool CompareOriginalSymbolsAliasTo(List<Symbol> symbols);
        bool CompareOriginalConfigsValueTo(List<Config> configs);
        void SetOriginalSymbolsTo(List<Symbol> symbols);
        void SetOriginalConfigsTo(List<Config> configs);
        List<Symbol> GetOriginalSymbolsValue();
        List<Config> GetOriginalConfigsValue();
        string GetOriginalConfigsValueByKey(string key);
        Symbol GetOriginalSymbolByAlias(string alias);
        string GetConfigsValueByKey(string key, List<Config> configs);
        int GetMaxLengthOfAlias();
    }
}
