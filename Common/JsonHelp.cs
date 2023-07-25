using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common
{
    public class JsonHelp : JsonResult
    {
        private new object Data { get; set; }
        public JsonHelp(object data)
        {
            this.Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //为空则抛出错误
            if (context == null)
            {
                throw new AccessViolationException("context");
            }
            var response = context.HttpContext.Response;
            //改写对象类型
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            //内容编码进行判断
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            var camelCaseSerializerSettings = new JsonSerializerSettings
            {
                //小写字母开头的驼峰命名
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss"//时间格式
             };
            response.Write(JsonConvert.SerializeObject(Data,camelCaseSerializerSettings));
        }
    }
}
