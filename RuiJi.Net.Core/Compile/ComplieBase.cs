namespace RuiJi.Net.Core.Compile
{
    /// <summary>
    /// Compile Base Class
    /// </summary>
    /// <typeparam name="T1">IProivder</typeparam>
    /// <typeparam name="T2">ICompile</typeparam>
    /// <typeparam name="T3">get result param generic</typeparam>
    public abstract class ComplieBase<T1,T2,T3> where T1 : IProvider, new() where T2 : ICompile, new()
    {
        protected T1 Provider { get; set; }

        protected T2 Compile { get; set; }

        public ComplieBase()
        {
            Provider = new T1();
            Compile = new T2();
        }

        /// <summary>
        /// get function code by function name
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>function code body</returns>
        protected virtual string GetFunc(string name)
        {
            return Provider.GetFunc(name);
        }

        /// <summary>
        /// get compile result
        /// </summary>
        /// <param name="t">param</param>
        /// <returns>execute result</returns>
        public abstract object[] GetResult(T3 t);
    }
}