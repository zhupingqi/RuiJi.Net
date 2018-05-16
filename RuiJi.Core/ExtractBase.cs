using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core
{
    public abstract class ExtractBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("selectors")]
        public List<ISelector> Selectors { get; set; }

        [JsonProperty("metas")]
        public Dictionary<string, List<ISelector>> Metas { get; set; }

        protected ExtractBase(string name = "")
        {
            Name = name;
            Selectors = new List<ISelector>();
            Metas = new Dictionary<string, List<ISelector>>();
        }

        public void AddMeta(string name, List<ISelector> selectors)
        {
            if (Metas.ContainsKey(name))
                Metas[name] = selectors;
            else
                Metas.Add(name, selectors);
        }
    }
}