using Newtonsoft.Json;
using RuiJi.Net.Core.Extractor.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Selector
{
    /// <summary>
    /// exclude selector
    /// </summary>
    public class ExcludeSelector : SelectorBase
    {
        /// <summary>
        /// exclude regex pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// exclude type enum
        /// </summary>
        [JsonProperty("etype")]
        public ExcludeTypeEnum Type { get; set; }

        /// <summary>
        /// set selector type enum
        /// </summary>
        /// <returns>select type</returns>
        protected override SelectorTypeEnum SetSelectType()
        {
            return SelectorTypeEnum.EXCLUDE;
        }

        public override string ToString()
        {
            var cmd = "ex";
            var exp = "";
            var remove = Remove ? "-r" : "";

            switch (Type)
            {
                case ExcludeTypeEnum.BEGIN:
                    {
                        exp = "ex /" + Pattern + "/ -b";
                        break;
                    }
                case ExcludeTypeEnum.END:
                    {
                        exp = "ex /" + Pattern + "/ -b";
                        break;
                    }
                case ExcludeTypeEnum.ALL:
                    {
                        exp = "ex /" + Pattern + "/ -b";
                        break;
                    }
            }

            return string.Join(" ", cmd, exp, remove);
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ExcludeSelector()
        { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type">exclude type enum</param>
        public ExcludeSelector(ExcludeTypeEnum type = ExcludeTypeEnum.BEGIN)
        { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pattern">exclude pattern</param>
        /// <param name="type">exclude type enum</param>
        public ExcludeSelector(string pattern,ExcludeTypeEnum type = ExcludeTypeEnum.BEGIN)
        {
            this.Pattern = pattern;
            this.Type = type;
        }
    }
}