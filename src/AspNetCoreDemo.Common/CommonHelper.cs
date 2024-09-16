using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Dtos;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace AspNetCoreDemo.Common
{
    public static class CommonHelper
    {
        #region 生成验证码图片
        /// <summary>  
        /// 生成指定长度的随机字符串 
        /// </summary>  
        /// <param name="codeLength">字符串的长度</param>  
        /// <returns>返回随机数字符串</returns>  
        public static string RndomStr(int codeLength)
        {
            //组成字符串的字符集合  0-9数字
            string chars = "0,1,2,3,4,5,6,7,8,9";

            string[] charArray = chars.Split(new Char[] { ',' });
            string code = "";
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数  
            Random rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (int i = 1; i < codeLength + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类  
                }
                int t = rand.Next(10);
                if (temp == t)
                {
                    return RndomStr(codeLength);//如果获取的随机数重复，则递归调用  
                }
                temp = t;//把本次产生的随机数记录起来  
                code += charArray[t];//随机数的位数加一  
            }
            return code;
        }

        /// <summary>  
        /// 将生成的字符串写入图像文件
        /// </summary>  
        /// <param name="code">验证码字符串</param>
        /// <param name="length">生成位数（默认4位）</param>  

        public static MemoryStream CreateValiCode(out string code, int length = 4)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            code = RndomStr(length);
            Bitmap Img = null;
            Graphics graphics = null;
            MemoryStream ms = null;
            Random random = new Random();
            //颜色集合  
            Color[] color = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            //定义图像的大小，生成图像的实例  

            Img = new Bitmap((int)code.Length * 18, 32);

            graphics = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象    
            graphics.Clear(Color.White);//背景设为白色  
            //在随机位置画背景点  

            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(Img.Width);
                int y = random.Next(Img.Height);
                graphics.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }
            //验证码绘制在graphics中  
            for (int i = 0; i < code.Length; i++)
            {
                int colorIndex = random.Next(7);//随机颜色索引值  
                int fontIndex = random.Next(4);//随机字体索引值  
                Font font = new Font(fonts[fontIndex], 15, FontStyle.Bold);//字体  
                Brush brush = new SolidBrush(color[colorIndex]);//颜色  
                int y = 4;
                if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                {
                    y = 2;
                }
                graphics.DrawString(code.Substring(i, 1), font, brush, 3 + (i * 12), y);//绘制一个验证字符  
            }
            ms = new MemoryStream();//生成内存流对象  
            Img.Save(ms, ImageFormat.Png);//将此图像以Png图像文件的格式保存到流中  
            graphics.Dispose();
            Img.Dispose();
#pragma warning restore CA1416 // 验证平台兼容性
            return ms;
        }

        #endregion

        #region 深拷贝
        /// <summary>
        /// 深拷贝对象 (XML方式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>加密后字符串</returns>
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        ///  短信通道clientId+时间戳加密
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static string GetSecurityKey(string clientId)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + clientId))).Replace("-", null).ToLower();
        }
        #endregion

        #region 获取IP
        /// <summary>
        /// 获取Ip
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            var httpContext = CustomHttpContext.Current;
            string ip = httpContext.Connection.RemoteIpAddress.ToString();

            // 存在并设置Nginx时，获取Nginx传递的ip参数
            if (httpContext.Request.Headers.Keys.Contains("X-Forwarded-For")
                && !string.IsNullOrWhiteSpace(httpContext.Request.Headers["X-Forwarded-For"]))
            {
                ip = httpContext.Request.Headers["X-Forwarded-For"].ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else if (httpContext.Request.Headers.Keys.Contains("X-Real-IP")
                && !string.IsNullOrWhiteSpace(httpContext.Request.Headers["X-Real-IP"]))
            {
                ip = httpContext.Request.Headers["X-Real-IP"].ToString();
            }

            return ip;
        }
        #endregion
    }
}
