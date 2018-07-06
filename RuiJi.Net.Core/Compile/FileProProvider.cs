using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// file selector processor provider,load selector processor define from disk
    /// </summary>
    public class FileProProvider : FileProviderBase
    {
        /// <summary>
        /// load selector processors
        /// </summary>
        /// <returns>selector processor dictionary</returns>
        public override Dictionary<string, string> Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funcs");

            var functions = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(path))
            {
                if (!file.EndsWith(".pro"))
                    continue;

                var key = file.Substring(file.LastIndexOf(@"\") + 1).Replace(".pro", "");
                var func = File.ReadAllText(file);

                functions.Add(key, "var content = \"{0}\";\n" + func);
            }

            return functions;
        }
    }
}
