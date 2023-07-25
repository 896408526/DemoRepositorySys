using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.DTO;

namespace IBLL
{
    public interface IMenuInfoBLL
    {
        List<GetMenuInfoDTO> GetMenuInfoes(int page, int limit, string title, out int count);

        bool CreateMenuInfo(MenuInfo menu, out string msg);

        bool DeleteMenuInfo(string id);

        bool DeleteListMenuInfo(List<string> ids);

        bool UpdateMenuInfo(MenuInfo menu, out string msg);

        MenuInfo GetMenuInfoById(string id);

        object GetSelectOption();

        /// <summary>
        /// 获取全部菜单的方法
        /// </summary>
        /// <returns></returns>
        List<GetMenuInfoDTO> GetMenuInfoDTOs();

        /// <summary>
        /// 获取用户可用菜单
        /// </summary>
        List<HomeMenuInfoDTO> GetMenus(string userId);
    }
}
