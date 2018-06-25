using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    public class ExtractResultCollection : List<ExtractResult>
    {
        public List<ExtractResult> Results
        {
            get {
                return this;
            }
        }

        public ExtractResultCollection()
        {
            
        }

        public string[] AllKeys
        {
            get
            {
                return this.Select(m => m.Name).Distinct().ToArray();
            }
        }

        public ExtractResult this[string name]
        {
            get
            {
                return this.SingleOrDefault(m => m.Name == name);
            }
        }

        public void Add(ExtractResultCollection collection)
        {
            foreach (var r in collection)
            {
                Add(r);
            }
        }

        public bool HasKeys()
        {
            return AllKeys.Length > 0;
        }
    }
}
