using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RuiJi.Net.Core.Compile
{
    public class FileJsFuncProvider : FileProviderBase
    {
        /// <summary>
        /// load js functions
        /// </summary>
        /// <returns>functions dictionary</returns>
        public override Dictionary<string, string> Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funcs_js");

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
