using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    public class ExtractMetaCollection : Dictionary<string, ExtractBase>
    {
        public void AddMeta(ExtractBase selectors)
        {
            if (ContainsKey(selectors.Name))
                this[selectors.Name] = selectors;
            else
                this.Add(selectors.Name, selectors);
        }

        public void AddMeta(string name, List<ISelector> selectors)
        {
            var ext = new ExtractBase(name);
            ext.Selectors = selectors;

            AddMeta(ext);
        }
    }
}