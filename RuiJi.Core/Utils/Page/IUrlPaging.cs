using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuiJi.Core.Utils.Page
{
    public interface IUrlPaging
    {
        string GetQuery(int? page);
    }
}
