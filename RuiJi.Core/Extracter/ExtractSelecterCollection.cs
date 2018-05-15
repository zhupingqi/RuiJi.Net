using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter
{
    public class ExtractSelecterCollection
    {
        private Dictionary<string, ExtractSelector> selectors;

        [JsonProperty("selectors")]
        public List<ExtractSelector> Selectors
        {
            set;
            get;
        }

        public ExtractSelecterCollection()
        {
            selectors = new Dictionary<string, ExtractSelector>();
            Selectors = new List<ExtractSelector>();
        }

        public ExtractSelecterCollection(ExtractSelecterCollection selectCollection)
        {
            this.selectors = selectCollection.selectors;
        }

        public string[] AllKeys
        {
            get
            {
                return selectors.Keys.ToArray();
            }
        }

        public ExtractSelector this[string name]
        {
            get
            {
                return selectors[name];
            }
        }

        public void Add(ExtractSelector selectRule)
        {
            selectors.Add(selectRule.Name, selectRule);
            Selectors.Add(selectRule);
        }

        public void Add(string name, List<ISelector> selectors)
        {
            var selector = new ExtractSelector()
            {
                Name = name,
                Selectors = selectors
            };

            Add(selector);
        }

        public void Update(ExtractSelector selector)
        {
            selectors[selector.Name] = selector;
        }

        public void Clear()
        {
            selectors.Clear();
        }

        public bool HasKeys()
        {
            return AllKeys.Length > 0;
        }

        public void Remove(string name)
        {
            selectors.Remove(name);
        }
    }
}