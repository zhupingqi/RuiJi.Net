using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Utils
{
    public class DecodeResult
    {
        public string Body { get; private set; }

        public string CharSet { get; private set; }

        public byte[] Raw { get; private set; }

        public DecodeResult(string charset,string body)
        {
            this.CharSet = charset;
            this.Body = body;
        }

        public DecodeResult(string charset, byte[] raw)
        {
            this.CharSet = charset;
            this.Raw = raw;
        }
    }
}
