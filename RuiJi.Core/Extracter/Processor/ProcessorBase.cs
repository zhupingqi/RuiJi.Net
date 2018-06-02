using CsQuery;
using RuiJi.Core.Extracter;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter.Processor
{
    public abstract class ProcessorBase : IProcessor
    {
        public ProcessResult Process(ISelector selector, ProcessResult result)
        {
            if (selector.Remove)
                return ProcessRemove(selector, result);
            else
                return ProcessNeed(selector, result);
        }

        public abstract ProcessResult ProcessNeed(ISelector selector, ProcessResult result);

        public abstract ProcessResult ProcessRemove(ISelector selector, ProcessResult result);
    }
}