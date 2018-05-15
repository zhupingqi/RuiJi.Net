﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter
{
    public class ProcessResult
    {
        public List<string> Matches { get; set; }

        public string Html
        {
            get
            {
                return string.Join("\r\n",Matches.ToArray());
            }
        }

        public ProcessResult()
        {
            Matches = new List<string>();
        }
    }
}