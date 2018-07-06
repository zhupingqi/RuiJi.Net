using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// file function provider,load functions define from disk
    /// </summary>
    public class FileFuncProvider : FileProviderBase
    {
        public FileFuncProvider()
        {
        }

        /// <summary>
        /// load functions
        /// </summary>
        /// <returns>functions dictionary</returns>
        public override Dictionary<string, string> Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funcs");

            var functions = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(path))
            {
                if (!file.EndsWith(".fun"))
                    continue;

                var key = file.Substring(file.LastIndexOf(@"\") + 1).Replace(".fun", "");
                var func = File.ReadAllText(file);

                functions.Add(key, func);
            }

            return functions;
        }
    }
}