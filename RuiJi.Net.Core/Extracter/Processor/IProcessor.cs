using CsQuery;
using RuiJi.Net.Core.Extracter;
using RuiJi.Net.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extracter.Processor
{
    public interface IProcessor
    {
        ProcessResult Process(ISelector selector, ProcessResult result);
    }
}