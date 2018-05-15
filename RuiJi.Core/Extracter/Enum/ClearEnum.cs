using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Enum
{
    public enum ClearEnum
    {
        NONE,
        IgnoreAll,
        IgnoreScript,
        IgnoreIframe,
        IgnoreEmptyDom,
        IgnoreInput,
        IgnoreTextarea,
        IgnoreComments,
        IgnoreForm,
        IgnoreSelect
    }
}