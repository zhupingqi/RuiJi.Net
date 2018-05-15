using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter.Processor
{
    public class XPathProcessor : ProcessorBase
    {
        public override ProcessResult ProcessNeed(ISelector sel, string html, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override ProcessResult ProcessRemove(ISelector sel, string html, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
