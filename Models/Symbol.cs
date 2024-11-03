using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSymbolConverter.Models
{
    public sealed class Symbol
    {
        public Symbol( string alias, string name)
        {
            Alias = alias;
            Name = name;
        }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
