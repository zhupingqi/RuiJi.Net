using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter
{
    public class ExtractBlockCollection : List<ExtractBlock>
    {
        public ExtractBlockCollection()
        {
            //selectors = new Dictionary<string, ExtractBlock>();
            //Blocks = new List<ExtractBlock>();
        }
    }
}