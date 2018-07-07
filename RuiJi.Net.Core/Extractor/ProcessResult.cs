using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor
{
    /// <summary>
    /// process result
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// matches content
        /// </summary>
        public List<string> Matches { get; set; }

        /// <summary>
        /// merged content
        /// </summary>
        public string Content
        {
            get
            {
                return string.Join("\r\n",Matches.ToArray());
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ProcessResult()
        {
            Matches = new List<string>();
        }
    }
}