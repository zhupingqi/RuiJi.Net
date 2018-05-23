using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace RuiJi.Core.Utils.Page
{
    public class WebPaging : _Paging, IUrlPaging
    {

        #region 分页参数
        private string _currentPageParam = "page";
        /// <summary>
        /// 分页参数
        /// </summary>
        public string Param
        {
            get
            {
                return _currentPageParam;
            }
            set
            {
                _currentPageParam = value;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造默认分页大小10 分页参数默认page
        /// </summary>
        public WebPaging()
            : this(10)
        {

        }

        /// <summary>
        /// 指定分页大小pageSize 分页参数默认page
        /// </summary>
        /// <param name="pageSize"></param>
        public WebPaging(int pageSize)
        {
            _currentPage = Convert.ToInt32(HttpContext.Current.Request.QueryString[Param]);
            _currentPage = _currentPage < 1 ? 1 : _currentPage;

            PageSize = pageSize;
        }

        /// <summary>
        /// 指定分页大小pageSize和分页参数Param
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="param"></param>
        public WebPaging(int pageSize, string param)
        {
            _currentPageParam = param;
            _currentPage = Convert.ToInt32(HttpContext.Current.Request.QueryString[Param]);
            _currentPage = _currentPage < 1 ? 1 : _currentPage;

            PageSize = pageSize;
        }

        /// <summary>
        /// 指定分页大小pageSize和当前页参数currentPage
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="param"></param>
        public WebPaging(string pageSize, string currentPage)
        {
            _currentPageParam = currentPage;
            _currentPage = Convert.ToInt32(HttpContext.Current.Request.QueryString[currentPage]);
            _currentPage = _currentPage < 1 ? 1 : _currentPage;

            PageSize = Convert.ToInt32(HttpContext.Current.Request.QueryString[pageSize]);
        }

        /// <summary>
        /// 指定分页大小pageSize和当前所在页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public WebPaging(int pageSize, int currentPage)
        {
            _currentPage = currentPage;
            PageSize = pageSize;
        }

        /// <summary>
        /// 指定分页大小pageSize和当前所在页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public WebPaging(int pageSize, int currentPage, string param)
        {
            _currentPage = currentPage < 1 ? 1 : currentPage; ;
            _currentPageParam = param;
            PageSize = pageSize;
        }
        #endregion

        #region 分页Url
        /// <summary>
        /// 分页Url
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetQuery(int? page)
        {
            NameValueCollection querys = new NameValueCollection(HttpContext.Current.Request.QueryString);
            querys.Remove(Param);

            if (page != null)
            {
                querys.Add(Param, page.ToString());
            }

            List<string> vals = new List<string>();
            foreach (string item in querys)
            {
                vals.Add(item + "=" + System.Web.HttpUtility.HtmlEncode(querys[item]));
            }

            string url = HttpContext.Current.Request.Url.AbsolutePath + "?" + String.Join("&", vals);

            return url;
        }
        #endregion
    }
}