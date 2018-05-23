using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace RuiJi.Core.Utils.Page
{
    public class AjaxPaging : _Paging, IUrlPaging
    {

        #region 分页参数
        private string _param = "page";
        /// <summary>
        /// 分页参数
        /// </summary>
        public string Param 
        {
            get
            {
                return _param;
            }
            set
            {
                _param = value;
            }            
        }

        public string Func
        {
            get;
            set;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造默认分页大小10 分页参数默认page
        /// </summary>
        public AjaxPaging() : this(10)
        {
            
        }

        /// <summary>
        /// 指定分页大小pageSize 分页参数默认page
        /// </summary>
        /// <param name="pageSize"></param>
        public AjaxPaging(int pageSize)
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
        public AjaxPaging(int pageSize, string param,string fun)
        {            
            _param = param;
            _currentPage = Convert.ToInt32(HttpContext.Current.Request.QueryString[Param]);
            _currentPage = _currentPage < 1 ? 1 : _currentPage;
            Func = fun;

            PageSize = pageSize;
        }

        /// <summary>
        /// 指定分页大小pageSize和当前所在页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public AjaxPaging(int pageSize, int currentPage)
        {
            _currentPage = currentPage;
            PageSize = pageSize;
        }

        /// <summary>
        /// 指定分页大小pageSize和当前所在页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public AjaxPaging(int pageSize, int currentPage, string param)
        {
            _currentPage = currentPage < 1 ? 1 : currentPage; ;
            _param = param;
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
                vals.Add(item + "=" + querys[item]);
            }

            string url = "javascript:" + Func + "(" + page.ToString() + ");";

            return url;
        }
        #endregion
    }
}