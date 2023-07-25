using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Common
{
    public class MD5help
    {
        public static string Md5(string txt)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] sor = Encoding.UTF8.GetBytes(txt);
                //开始加密
                byte[] result = md5.ComputeHash(sor);
                //可变的string类型
                StringBuilder strbul = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    //加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
                    strbul.Append(result[i].ToString("x2"));
                }
                return strbul.ToString();
            }
        }
    }
}
