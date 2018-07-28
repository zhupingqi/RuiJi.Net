using System;
using System.Collections.Generic;
using System.Text;

namespace RuiJi.Net.Core.Code.Provider
{
    public interface ICodeProvider
    {
        /// <summary>
        /// get code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code</returns>
        string GetCode(string name);
    }
}
