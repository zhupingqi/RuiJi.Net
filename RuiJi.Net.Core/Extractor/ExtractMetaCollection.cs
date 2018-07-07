using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract meta collection
    /// </summary>
    public class ExtractMetaCollection : Dictionary<string, ExtractBase>
    {
        /// <summary>
        /// add meta
        /// </summary>
        /// <param name="selectors">meta selectors</param>
        public void AddMeta(ExtractBase selectors)
        {
            if (ContainsKey(selectors.Name))
                this[selectors.Name] = selectors;
            else
                this.Add(selectors.Name, selectors);
        }

        /// <summary>
        /// add meta
        /// </summary>
        /// <param name="name">meta name</param>
        /// <param name="selectors">selector list</param>
        public void AddMeta(string name, List<ISelector> selectors)
        {
            var ext = new ExtractBase(name);
            ext.Selectors = selectors;

            AddMeta(ext);
        }
    }
}