using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IConsumableInfoBLL
    {
        List<GetConsumableInfoDTO> GetConsumableInfoes(int page, int limit, string consumableName, out int count);

        bool CreateConsumableInfo(ConsumableInfo cons, out string msg);

        bool DeleteConsumableInfo(string id);

        bool DeleteListConsumableInfo(List<string> ids);

        bool UpdateConsumableInfo(ConsumableInfo cons, out string msg);

        ConsumableInfo GetConsumableInfoById(string id);

        object GetSelectOption();

        /// <summary>
        /// Excel耗材入库方法
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="userId">用户ID</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        bool UpLoad(Stream stream, string extension,string userId,out string msg);

       /// <summary>
       /// 导出耗材记录表
       /// </summary>
       /// <param name="downLoadName"></param>
       /// <returns></returns>
        Stream GetDownLoad(out string downLoadName);
    }
}
