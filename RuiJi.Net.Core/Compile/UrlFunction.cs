using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// url function from url
    /// </summary>
    public class UrlFunction
    {
        /// <summary>
        /// function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// function args
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// function index at url
        /// </summary>
        public int Index { get; set; }
    }
}