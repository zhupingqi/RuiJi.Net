using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Utils
{
    public class RuiJiExpParser
    {
        public RuiJiExpParser()
        {

        }

        public object ParseFile(string file)
        {
            if (!File.Exists(file))
                return null;

            return Parse(File.ReadAllLines(file));
        }

        public object Parse(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return null;

            return Parse(expression.Replace("\r\n","\n").Split('\n'));
        }

        public object Parse(string[] lines)
        {
            return null;
        }
    }
}