using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter
{
    public class ExtractBlockCollection
    {
        private Dictionary<string, ExtractBlock> selectors;

        [JsonProperty("selectors")]
        public List<ExtractBlock> Blocks
        {
            set;
            get;
        }

        public int Count
        {
            get
            {
                return Blocks.Count;
            }
        }

        public ExtractBlockCollection()
        {
            selectors = new Dictionary<string, ExtractBlock>();
            Blocks = new List<ExtractBlock>();
        }

        public ExtractBlockCollection(ExtractBlockCollection selectCollection)
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

        public ExtractBlock this[string name]
        {
            get
            {
                return selectors[name];
            }
        }

        public void Add(ExtractBlock selectRule)
        {
            selectors.Add(selectRule.Name, selectRule);
            Blocks.Add(selectRule);
        }

        public void Add(string name, List<ISelector> selectors)
        {
            var selector = new ExtractBlock()
            {
                Name = name,
                Selectors = selectors
            };

            Add(selector);
        }

        public void Update(ExtractBlock selector)
        {
            selectors[selector.Name] = selector;
        }

        public bool ContainsKey(string name)
        {
            return AllKeys.Contains(name);
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