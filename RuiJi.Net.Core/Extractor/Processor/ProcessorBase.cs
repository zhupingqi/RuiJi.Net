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
    public abstract class ProcessorBase<T> : IProcessor where T : ISelector
    {
        /// <summary>
        /// process base method
        /// </summary>
        /// <param name="selector">selector interface</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public ProcessResult Process(ISelector selector, ProcessResult result)
        {
            if (selector.Remove)
                return ProcessRemove((T)selector, result);
            else
                return ProcessNeed((T)selector, result);
        }

        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">selector interface</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public abstract ProcessResult ProcessNeed(T selector, ProcessResult result);

        /// <summary>
        /// process need
        /// </summary>
        /// <param name="selector">selector interface</param>
        /// <param name="result">pre process result</param>
        /// <returns>new process result</returns>
        public abstract ProcessResult ProcessRemove(T selector, ProcessResult result);
    }
}