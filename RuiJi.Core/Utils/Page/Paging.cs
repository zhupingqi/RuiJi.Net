using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;


namespace RuiJi.Core.Utils.Page
{
    public class Paging : _Paging
    {
        #region 构造函数
        /// <summary>
        /// 构造默认分页大小
        /// </summary>
        public Paging()
            : this(10, 1)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public Paging(int pageSize, int currentPage)
        {
            _currentPage = currentPage;
            PageSize = pageSize;
        }

        #endregion
    }
}