using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RuiJi.Core.Extracter.Selector;

namespace RuiJi.Core.Extracter
{
    public class ExtractSelector
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hasTile")]
        public bool HasTile
        {
            get
            {
                return TileSelectors.Count > 0;
            }
        }

        [JsonProperty("selectors")]
        public List<ISelector> Selectors { get; set; }

        [JsonProperty("tileSelectors")]
        public List<ExtractSelector> TileSelectors { get; set; }

        public ExtractSelector()
        {
            Selectors = new List<ISelector>();
            TileSelectors = new List<ExtractSelector>();
        }

        public ExtractSelector(ExtractSelector select)
        {
            this.Name = select.Name;
            this.Selectors = select.Selectors;
            this.TileSelectors = select.TileSelectors;
        }
    }
}