using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// extract result collection
    /// </summary>
    public class ExtractResultCollection : List<ExtractResult>
    {
        /// <summary>
        /// extract results
        /// </summary>
        public List<ExtractResult> Results
        {
            get {
                return this;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExtractResultCollection()
        {
            
        }

        /// <summary>
        /// extract keys
        /// </summary>
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
