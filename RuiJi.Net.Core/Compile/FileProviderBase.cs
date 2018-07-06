using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// file provider base class
    /// </summary>
    public abstract class FileProviderBase : IProvider
    {
        /// <summary>
        /// functions dictionary
        /// </summary>
        public Dictionary<string, string> Functions { get; protected set; }

        public FileProviderBase()
        {
            Functions = Load();
        }

        /// <summary>
        /// load function abstract method
        /// </summary>
        /// <returns>functions dictionary</returns>
        public abstract Dictionary<string, string> Load();

        /// <summary>
        /// get function code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code</returns>
        public virtual string GetFunc(string name)
        {
            if (!Functions.Keys.Contains(name))
                return "";

            return Functions[name];
        }
    }
}
