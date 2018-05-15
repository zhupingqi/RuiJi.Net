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
        public ProcessResult Process(ISelector sel, string html, params object[] args)
        {
            if (sel.Remove)
                return ProcessRemove(sel, html, args);
            else
                return ProcessNeed(sel, html, args);
        }

        public abstract ProcessResult ProcessNeed(ISelector sel, string html, params object[] args);

        public abstract ProcessResult ProcessRemove(ISelector sel, string html, params object[] args);
    }
}