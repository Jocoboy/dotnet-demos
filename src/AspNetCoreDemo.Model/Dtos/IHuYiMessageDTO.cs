using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    /// <summary>
    /// 互亿无限PostData
    /// 通过POST、 GET 方式进行短信提交，数据编码采用utf-8格式，请求地址为：https://106.ihuyi.com/webservice/sms.php?method=Submit
    /// 在线调试网址：https://user.ihuyi.com/new/console
    /// </summary>
    public class IHuYiMessageDto
    {
        /// <summary>
        /// APIID(用户中心【验证码通知短信】-【产品纵览】查看)
        /// </summary>
        public string account;

        /// <summary>
        /// 1、APIKEY(用户中心【验证码通知短信】-【产品纵览】查看)
        /// 2、动态密码(生成动态密码方式请看该文档末尾的说明)
        /// </summary>
        public string password;

        /// <summary>
        /// 接收手机号码，只能提交1个号码
        /// </summary>
        public string mobile;

        /// <summary>
        /// 短信内容(编码格式为 UTF-8，支持 300 个字的长短信，长短信 按多条计费)
        /// 例如:您的验证码是:1234。请不要把验证码泄露给其他人。
        /// </summary>
        public string content;

        /// <summary>
        /// Unix时间戳（10位整型数字，当使用动态密码方式时为必填）
        /// </summary>
        public string time;

        /// <summary>
        /// 返回格式（可选值为：xml或json，系统默认为xml）
        /// </summary>
        public string format;
    }
}
