using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RuiJi.Core.Extracter
{
    public class ExtractRequest
    {
        public ExtractBlock Block { get; set; }

        //[JsonProperty("content")]
        public string Content { get; set; }
    }
}
