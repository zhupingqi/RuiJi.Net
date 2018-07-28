using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Code.Provider
{
    public class FileCodeProvider : ICodeProvider
    {
        private Dictionary<string, string> codes;

        public FileCodeProvider(string path, string ext)
        {
            codes = new Dictionary<string, string>();

            foreach (var file in Directory.GetFiles(path))
            {
                if (!file.EndsWith("." + ext))
                    continue;

                FileInfo fileInfo = new FileInfo(file);

                var key = fileInfo.Name;
                key = key.Substring(0, key.Length - fileInfo.Extension.Length);

                var func = File.ReadAllText(file);

                codes.Add(key, func);
            }
        }

        public string GetCode(string name)
        {
            if (!codes.Keys.Contains(name))
                return "";

            return codes[name];
        }
    }
}