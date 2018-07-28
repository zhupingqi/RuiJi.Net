using System;


namespace RuiJi.Net.Core.Utils.Page
{
    public abstract class _Paging
    {
        #region 总数
        public int _count;
        /// <summary>
        /// 总数
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                _pages = (_count + PageSize - 1) / PageSize;
            }
        }
        #endregion

        #region 当前页
        protected int _currentPage;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
            }
        }
        #endregion

        #region 总页数
        private int _pages;
        /// <summary>
        /// 总页数
        /// </summary>
        public int Pages
        {
            get
            {
                return (Count + PageSize - 1) / PageSize;
            }
        }
        #endregion

        #region 分页大小
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }
        #endregion        

        #region 首页和尾页判断
        /// <summary>
        /// 是否是第一页
        /// </summary>
        public bool IsFirst
        {
            get
            {
                return _currentPage <= 1 ? true : false;
            }
        }

        /// <summary>
        /// 是否是最后一页
        /// </summary>
        public bool IsLast
        {
            get
            {
                return _currentPage >= Pages ? true : false;
            }
        }
        #endregion

        #region 获取当前页记录的开始和结尾
        public int Start
        {
            get
            {
                return (_currentPage - 1) * PageSize;
            }
        }

        public int End
        {
            get
            { 
                int end = _currentPage * PageSize;
                return end > _count ? _count : end;
            }
        }

        #endregion
    }
}