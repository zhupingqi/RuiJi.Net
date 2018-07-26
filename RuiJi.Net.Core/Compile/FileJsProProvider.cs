using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RuiJi.Net.Core.Compile
{
    public class FileJsProProvider : FileProviderBase
    {
        /// <summary>
        /// load js selector processors
        /// </summary>
        /// <returns>selector processor dictionary</returns>
        public override Dictionary<string, string> Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funcs_js");

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
