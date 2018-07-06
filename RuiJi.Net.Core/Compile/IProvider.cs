using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// provider interface
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// functions dictionary
        /// </summary>
        Dictionary<string, string> Functions { get; }

        /// <summary>
        /// load method
        /// </summary>
        /// <returns>functions dictionary</returns>
        Dictionary<string, string> Load();

        /// <summary>
        /// get code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code</returns>
        string GetFunc(string name);
    }
}
