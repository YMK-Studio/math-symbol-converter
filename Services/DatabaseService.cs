using MathSymbolConverter.Models;
using MathSymbolConverter.Utilities;
using MathSymbolConverter.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static ImTools.ImMap;

namespace MathSymbolConverter.Services
{
    public class DatabaseService : IDatabaseService
    {
        private List<Symbol> _symbols;
        public List<Symbol> Symbols
        {
            get => _symbols; 
        }

        private List<Config> _configs;
        public List<Config> Configs
        {
            get => _configs;
        }

        public bool InitializeOriginalSymbols()
        {
            _symbols = new();
            foreach (string str in Properties.Settings.Default.SymbolAliasList)   // ex. (Greek_lc_alpha.svg.png, alp, α)
            {
                string[] words = str.Split('\t');
                _symbols.Add(new(words[0], words[1]));
            }
            return true;
        }

        public bool InitializeOriginalConfigs()
        {
            _configs = new();
            foreach (string str in Properties.Settings.Default.ConfigKeyValueList)
            {
                string[] words = str.Split("\t");   // Size 1
                _configs.Add(new(words[0], words[1]));
            }
            return true;
        }

        public void SaveOriginalSymbols()
        {
            Properties.Settings.Default.SymbolAliasList.Clear();
            foreach (Symbol s in _symbols)
            {
                Properties.Settings.Default.SymbolAliasList.Add($"{s.Alias}\t{s.Name}");
            }
            Properties.Settings.Default.Save();
        }

        public void SaveOriginalConfigs()
        {
            Properties.Settings.Default.ConfigKeyValueList.Clear();
            foreach (Config c in _configs)
            {
                Properties.Settings.Default.ConfigKeyValueList.Add($"{c.Key}\t{c.Value}");
            }
            Properties.Settings.Default.Save();
        }

        public bool CompareOriginalSymbolsAliasTo(List<Symbol> symbols)
        {
            for(int i = 0; i < symbols.Count; i++)
            {
                if (!symbols[i].Alias.Equals(_symbols[i].Alias))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CompareOriginalConfigsValueTo(List<Config> configs)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                if (!configs[i].Value.Equals(_configs[i].Value))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetOriginalSymbolsTo(List<Symbol> symbols)
        {
            _symbols = symbols.ConvertAll(s => new Symbol(s.Alias, s.Name));
        }

        public void SetOriginalConfigsTo(List<Config> configs)
        {
            _configs = configs.ConvertAll(c => new Config(c.Key, c.Value));
        }

        public List<Symbol> GetOriginalSymbolsValue()
        {
            return _symbols.ConvertAll(s => new Symbol(s.Alias, s.Name));
        }

        public List<Config> GetOriginalConfigsValue()
        {
            return _configs.ConvertAll(c => new Config(c.Key, c.Value));
        }

        public string GetOriginalConfigsValueByKey(string key)
        {
            foreach (Config c in _configs)
            {
                if (c.Key.Equals(key))
                {
                    return c.Value;
                }
            }
            return null;
        }

        public Symbol GetOriginalSymbolByAlias(string alias)
        {
            foreach (Symbol s in _symbols)
            {
                if (s.Alias.Equals(alias))
                {
                    return s;
                }
            }
            return null;
        }

        public int GetMaxLengthOfAlias()
        {
            int maxLength = 0;
            foreach (Symbol s in _symbols)
            {
                if (s.Alias.Length > maxLength)
                    maxLength = s.Alias.Length;
            }
            return maxLength;
        }

        public string GetConfigsValueByKey(string key, List<Config> configs)
        {
            foreach (Config c in configs)
            {
                if (c.Key.Equals(key))
                {
                    return c.Value;
                }
            }
            return null;
        }
    }
}
