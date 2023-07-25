using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IBLL
{
    public interface IRoleInfoBLL
    {
        List<GetRoleInfoDTO> GetRoleInfos(int page, int limit, string roleName, out int count);

        bool CreateRoleInfo(RoleInfo role, out string msg);

        bool DeleteRoleInfo(string id);

        bool DeleteListRolerInfo(List<string> ids);

        bool UpdateRoleInfo(RoleInfo role, out string msg);

        RoleInfo GetRoleInfoById(string id);

        /// <summary>
        /// 获取绑定用户的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<string> GetBindUserList(string id);

        /// <summary>
        /// 绑定用户的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool BindUserInfo(List<string> userIds,string roleId);

        /// <summary>
        /// 获取绑定菜单的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<string> GetBindMenuList(string id);

        /// <summary>
        /// 绑定菜单的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool BindMenuInfo(List<string> menuIds, string roleId);
    }
}
