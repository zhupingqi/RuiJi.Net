using RuiJi.Net.Core.Extracter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter.Selector
{
    public interface ISelector
    {
        SelectorTypeEnum SelectorType { get; }

        bool Remove { get; set; }

        string Value { get; set; }
    }
}