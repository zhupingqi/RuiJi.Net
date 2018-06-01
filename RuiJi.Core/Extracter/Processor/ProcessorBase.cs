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
        public ProcessResult Process(ISelector selector, string content, params object[] args)
        {
            if (selector.Remove)
                return ProcessRemove(selector, content, args);
            else
                return ProcessNeed(selector, content, args);
        }

        public abstract ProcessResult ProcessNeed(ISelector selector, string content, params object[] args);

        public abstract ProcessResult ProcessRemove(ISelector selector, string content, params object[] args);
    }
}