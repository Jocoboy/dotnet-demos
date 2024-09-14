using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.ViewModels
{
    /// <summary>
    /// 分页返回结果 ViewModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageMessageViewModel<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Res { get; set; } = 0;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "SUCCESS";


        public PageResultViewModel<T> Data { get; set; }
    }

    /// <summary>
    /// 分页返回结果 ViewModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResultViewModel<T>
    {

        /// <summary>
        /// 当前页返回数据集合
        /// </summary>
        public List<T> List { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public long Count { get; set; }

    }
}
