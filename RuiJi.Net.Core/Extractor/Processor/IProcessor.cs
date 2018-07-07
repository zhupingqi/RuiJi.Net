using CsQuery;
using RuiJi.Net.Core.Extractor;
using RuiJi.Net.Core.Extractor.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Extractor.Processor
{
    /// <summary>
    /// processor interface
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// process method
        /// </summary>
        /// <param name="selector">selector interface</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        ProcessResult Process(ISelector selector, ProcessResult result);
    }
}