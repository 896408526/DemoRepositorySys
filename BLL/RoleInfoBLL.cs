using IBLL;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using IDAL;

namespace BLL
{
    public class RoleInfoBLL : IRoleInfoBLL
    {
        private IRoleInfoDAL _roleInfoDAL;
        private IR_UserInfo_RoleInfoDAL _r_UserInfo_RoleInfoDAL;
        private IR_RoleInfo_MenuInfoDAL _r_RoleInfo_MenuInfoDAL;
        private RepositorySysDBcontext _dbContext;

        public RoleInfoBLL(
            IRoleInfoDAL roleInfoDAL,
            IR_UserInfo_RoleInfoDAL r_UserInfo_RoleInfoDAL,
            IR_RoleInfo_MenuInfoDAL r_RoleInfo_MenuInfoDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _roleInfoDAL = roleInfoDAL;
            _r_UserInfo_RoleInfoDAL = r_UserInfo_RoleInfoDAL;
            _r_RoleInfo_MenuInfoDAL = r_RoleInfo_MenuInfoDAL;
            _dbContext = dbContext;
        }

        #region 获取部门表的方法 (GetDepartmentInfos)
        /// <summary>
        /// 获取部门表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="roleName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetRoleInfoDTO> GetRoleInfos(int limit, int page, string roleName, out int count)
        {
            var data = _roleInfoDAL.GetEntities()
                       .Where(r => r.IsDelete == false)
                       .Select(r => new GetRoleInfoDTO {
                           Id = r.Id,
                           RoleName = r.RoleName,
                           Description = r.Description,
                           CreateTime = r.CreateTime,
                           IsDelete = r.IsDelete
                       });

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                //条件精准查询
                data = data.Where(d => d.RoleName == roleName);
            }
            //计算数据总数
            count = data.Count();
            //分页(降序)
            return data.OrderByDescending(it => it.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();
        }
        #endregion

        #region 添加方法 (CreateRoleInfo)
        /// <summary>
        /// 添加的方法
        /// </summary>
        /// <param name="role"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CreateRoleInfo(RoleInfo role, out string msg)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                msg = "角色名不能为空";
                return false;
            }
            RoleInfo entity = _roleInfoDAL.GetEntityById(role.Id);
            if (_roleInfoDAL.GetEntities().FirstOrDefault(it => it.RoleName == role.RoleName) != null)
            {
                msg = "角色已经存在";
                return false;
            }
            role.Id = Guid.NewGuid().ToString();
            role.CreateTime = DateTime.Now;
            bool isSuccess = _roleInfoDAL.CreateEntity(role);
            msg = isSuccess ? "添加成功" : "添加失败";
            return isSuccess;
        }
        #endregion

        #region 删除方法 (DeleteRoleInfo)
        /// <summary>
        /// 删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteRoleInfo(string id)
        {
            RoleInfo role = _roleInfoDAL.GetEntityById(id);
            if (role == null)
            {
                return false;
            }
            role.IsDelete = true;
            role.DeleteTime = DateTime.Now;
            _roleInfoDAL.UpdateEntity(role);
            return true;
        }
        #endregion

        #region 批量删除的方法 (DeleteListRoleInfo)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteListRolerInfo(List<string> ids)
        {
            List<RoleInfo> roleList = _roleInfoDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {   
                RoleInfo role = _roleInfoDAL.GetEntityById(item);
                if (roleList == null)
                {
                    return false;
                }
                role.IsDelete = true;
                role.DeleteTime = DateTime.Now;

                _roleInfoDAL.UpdateEntity(role);
            }
            return true;
        }
        #endregion

        #region 修改的方法 (UpdateRoleInfo)
        /// <summary>
        /// 修改角色的方法
        /// </summary>
        /// <param name="role"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateRoleInfo(RoleInfo role, out string msg)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                msg = $"角色名不能为空";
                return false;
            }
            RoleInfo entity = _roleInfoDAL.GetEntityById(role.Id);
            if (entity.Id == null)
            {
                msg = $"角色不存在";
                return false;
            }
            //判断重复
            if (entity.RoleName != role.RoleName)
            {
                var data = _roleInfoDAL.GetEntities().FirstOrDefault(it => it.RoleName == role.RoleName);
                if (data != null)
                {
                    msg = "角色名已经被占用";
                    return false;
                }
            }
            entity.RoleName = role.RoleName;
            entity.Description = role.Description;

            bool IsSuccess = _roleInfoDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改用户失败";

            return IsSuccess;
        }
        #endregion

        #region 根据ID获取数据返回赋值 (GetDepartmentInfoById)
        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoleInfo GetRoleInfoById(string id)
        {
            return _roleInfoDAL.GetEntityById(id);
        }
        #endregion

        #region 获取当前用户绑定的用户列表 (GetBindUserList)
        /// <summary>
        /// 获取当前用户绑定的用户列表
        /// </summary>
        /// <param name="id">当前角色的ID</param>
        /// <returns></returns>
        public List<string> GetBindUserList(string id)
        {
            List<string> userList = _r_UserInfo_RoleInfoDAL.GetEntities()
                                                            .Where(it => it.RoleId == id)
                                                            .Select(it => it.UserId)
                                                            .ToList();

            return userList;
        }
        #endregion

        #region 获取当前角色的绑定用户 (BindUserInfo)
        /// <summary>
        /// 获取当前角色的绑定用户
        /// </summary>
        /// <param name="userIds">用户ID集</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns> 
        public bool BindUserInfo(List<string> userIds, string roleId)
        {
            //获取选定ID，然后判断该ID和当前基础ID是否一致，一致则保留，不一致则绑定
            List<R_UserInfo_RoleInfo> roleBind = _r_UserInfo_RoleInfoDAL.GetEntities().Where(it => it.RoleId == roleId).ToList();

            //解绑
            foreach (var item in roleBind)
            {
                bool isHas = userIds == null ? false : userIds.Any(it => it == item.UserId);
                if (!isHas)
                {
                    _r_UserInfo_RoleInfoDAL.DeleteEntity(item);
                }
            }

            //截取解绑
            if (userIds == null || userIds.Count() == 0)
            {
                return true;
            }
            //绑定
            foreach (var item in userIds)
            {
                bool isHas = roleBind.Any(it => it.UserId == item);
                if (!isHas)
                {
                    R_UserInfo_RoleInfo entity = new R_UserInfo_RoleInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = item,
                        RoleId = roleId,
                        CreateTime = DateTime.Now
                    };
                    _r_UserInfo_RoleInfoDAL.CreateEntity(entity);
                }
            }

            return true;
        }
        #endregion

        #region 获取当前用户绑定的菜单列表 (GetBindMenuList)
        /// <summary>
        /// 获取当前用户绑定的菜单列表
        /// </summary>
        /// <param name="id">当前角色的ID</param>
        /// <returns></returns>
        public List<string> GetBindMenuList(string id)
        {
            List<string> userList = _r_RoleInfo_MenuInfoDAL.GetEntities()
                                                            .Where(it => it.RoleId == id)
                                                            .Select(it => it.MenuId)
                                                            .ToList();

            return userList;
        }
        #endregion

        #region 获取当前菜单的绑定用户 (BindMenuInfo)
        /// <summary>
        /// 获取当前菜单的绑定用户
        /// </summary>
        /// <param name="menuIds">用户ID集</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public bool BindMenuInfo(List<string> menuIds, string roleId)
        {
            //获取选定ID，然后判断该ID和当前基础ID是否一致，一致则保留，不一致则绑定
            List<R_RoleInfo_MenuInfo> roleBind = _r_RoleInfo_MenuInfoDAL.GetEntities().Where(it => it.RoleId == roleId).ToList();

            //解绑
            foreach (var item in roleBind)
            {
                bool isHas = menuIds == null ? false : menuIds.Any(it => it == item.MenuId);
                if (!isHas)
                {
                    _r_RoleInfo_MenuInfoDAL.DeleteEntity(item);
                }
            }

            //截取解绑
            if (menuIds == null || menuIds.Count() == 0)
            {
                return true;
            }
            //绑定
            foreach (var item in menuIds)
            {
                bool isHas = roleBind.Any(it => it.MenuId == item);
                if (!isHas)
                {
                    R_RoleInfo_MenuInfo entity = new R_RoleInfo_MenuInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        MenuId = item,
                        RoleId = roleId,
                        CreateTime = DateTime.Now
                    };
                    _r_RoleInfo_MenuInfoDAL.CreateEntity(entity);
                }
            }

            return true;
        }
        #endregion
    }
}
