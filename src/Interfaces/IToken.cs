using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sulfur.Enum;

namespace Sulfur.Interfaces
{
    public interface IToken
    {
        public string Value { get; set; }
        public TokenType Type { get; set; }
    }
}
