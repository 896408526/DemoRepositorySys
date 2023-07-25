using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model.DTO;
using Model;
using Common;

namespace BLL
{
    public class MenuInfoBLL : IMenuInfoBLL
    {
        private IMenuInfoDAL _menuInfoDAL;
        private IR_RoleInfo_MenuInfoDAL _r_RoleInfo_MenuInfoDAL;
        private IR_UserInfo_RoleInfoDAL _r_UserInfo_RoleInfoDAL;
        private RepositorySysDBcontext _dbContext;

        public MenuInfoBLL(
            IMenuInfoDAL menuInfoDAL
            , IR_RoleInfo_MenuInfoDAL r_RoleInfo_MenuInfoDAL
            , IR_UserInfo_RoleInfoDAL r_UserInfo_RoleInfoDAL
            , RepositorySysDBcontext dbContext
            )
        {
            _menuInfoDAL = menuInfoDAL;
            _r_RoleInfo_MenuInfoDAL = r_RoleInfo_MenuInfoDAL;
            _r_UserInfo_RoleInfoDAL = r_UserInfo_RoleInfoDAL;
            _dbContext = dbContext;
        }

        #region 获取菜单表的方法 (GetMenuInfoes)
        /// <summary>
        /// 获取菜单表的方法
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="title"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetMenuInfoDTO> GetMenuInfoes(int page, int limit, string title, out int count)
        {
            var data = from m in _dbContext.MenuInfo.Where(m => m.IsDelete == false)
                       join m2 in _dbContext.MenuInfo.Where(m => m.IsDelete == false)
                       on m.ParentId equals m2.Id
                       into mmtemp
                       from mm2 in mmtemp.DefaultIfEmpty()
                       select new GetMenuInfoDTO
                       {
                           Id = m.Id,
                           Title = m.Title,
                           Description = m.Description,
                           Level = m.Level,
                           Sort = m.Sort,
                           Href = m.Href,
                           ParentId = mm2.Title,
                           Icon = m.Icon,
                           Target = m.Target,
                           CreateTime = m.CreateTime
                       };

            if (!string.IsNullOrWhiteSpace(title))
            {
                //条件精准查询
                data = data.Where(r => r.Title == title);
            }
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderBy(it => it.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 添加方法 (CreateMenuInfo)
        public bool CreateMenuInfo(MenuInfo menu, out string msg)
        {
            if (string.IsNullOrWhiteSpace(menu.Title))
            {
                msg = "标题不能为空";
                return false;
            }
            if (_menuInfoDAL.GetEntities().FirstOrDefault(it => it.Title == menu.Title) !=null)
            {
                msg = "标题已经存在";
                return false;
            }
            menu.Id = Guid.NewGuid().ToString();
            menu.CreateTime = DateTime.Now;
            bool IsSuccess = _menuInfoDAL.CreateEntity(menu);
            msg = IsSuccess ? "添加成功" : "添加失败";
            return IsSuccess;
        }
        #endregion

        #region 删除方法 (DeleteMenuInfo)
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteMenuInfo(string id)
        {
            MenuInfo menu = _menuInfoDAL.GetEntityById(id);
            if (menu == null)
            {
                return false;
            }
            menu.IsDelete = true;
            menu.DeleteTime = DateTime.Now;
            _menuInfoDAL.UpdateEntity(menu);
            return true;
        }
        #endregion

        #region 修改的方法 (UpdateMenuInfo)
        /// <summary>
        /// 修改菜单的方法
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateMenuInfo(MenuInfo menu, out string msg)
        {
            if (string.IsNullOrWhiteSpace(menu.Title))
            {
                msg = $"标题不能为空";
                return false;
            }
            MenuInfo entity = _menuInfoDAL.GetEntityById(menu.Id);
            if (entity.Id == null)
            {
                msg = "ID不存在";
                return false;
            }
            //判断重复
            if (entity.Title != menu.Title)
            {
                var data = _menuInfoDAL.GetEntities().FirstOrDefault(it => it.Title == menu.Title);
                if (data != null)
                {
                    msg = "标题名已经被占用";
                    return false;
                }
            }

            entity.Title = menu.Title;
            entity.Description = menu.Description;
            entity.Level = menu.Level;
            entity.Sort = menu.Sort;
            entity.Href = menu.Href;
            entity.ParentId = menu.ParentId;
            entity.Icon = menu.Icon;
            entity.Target = menu.Target;

            bool IsSuccess = _menuInfoDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改用户失败";

            return IsSuccess;
        }
        #endregion

        #region 批量删除的方法 (DeleteListMenuInfo)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteListMenuInfo(List<string> ids)
        {
            List<MenuInfo> deptList = _menuInfoDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                MenuInfo menu = _menuInfoDAL.GetEntityById(item);
                if (deptList == null)
                {
                    return false;
                }
                menu.IsDelete = true;
                menu.DeleteTime = DateTime.Now;

                _menuInfoDAL.UpdateEntity(menu);
            }
            return true;
        }
        #endregion

        #region 根据ID获取数据返回赋值 (GetMenuInfoById)
        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MenuInfo GetMenuInfoById(string id)
        {
            return _menuInfoDAL.GetEntityById(id);
        }
        #endregion

        #region 获取数据库列表赋值下拉框 (GetSelectOption)
        /// <summary>
        /// 获取主管和父级部门下拉框的方法
        /// </summary>
        /// <returns></returns>
        public object GetSelectOption()
        {
            var menuData = _dbContext.MenuInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.Title,
            });

            return new
            {
                menuData = menuData
            };
        }
        #endregion

        #region 获取全部菜单的方法 (GetMenuInfoDTOs)
        /// <summary>
        /// 获取全部菜单的方法
        /// </summary>
        /// <returns></returns>
        public List<GetMenuInfoDTO> GetMenuInfoDTOs()
        {
            var data = _menuInfoDAL.GetEntities()
                                    .Where(it => it.IsDelete == false)
                                    .Select(it => new GetMenuInfoDTO
                                    {
                                        Id = it.Id,
                                        Title = it.Title
                                    }).ToList();

            return data;
        }
        #endregion

        #region 根据用户ID获取用户可以访问的菜单 (GetMenus)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<HomeMenuInfoDTO> GetMenus(string userId)
        {
            //获取角色
            List<string> roleIds = _r_UserInfo_RoleInfoDAL.GetEntities().Where(it => it.UserId == userId)
                .Select(it=>it.RoleId).ToList();

            //获取角色绑定的菜单
            List<string> menuIds = _r_RoleInfo_MenuInfoDAL.GetEntities().Where(it => roleIds.Contains(it.RoleId))
                .Select(it => it.MenuId).ToList();

            //查询角色绑定的菜单升序
            List<MenuInfo> allMenus = _menuInfoDAL.GetEntities().Where(it => it.IsDelete == false && menuIds.Contains(it.Id))
                .OrderBy(it => it.Sort).ToList();

            List<HomeMenuInfoDTO> topMenus = allMenus.Where(it => it.Level == 1).OrderBy(it => it.Sort).Select(it => new HomeMenuInfoDTO()
            {
                Id = it.Id,
                PId = it.ParentId,
                Title = it.Title,
                Href = it.Href,
                Icon = it.Icon
            }).ToList();


            GetMenuChild(topMenus, allMenus);

            #region  old
            ////找顶级菜单的子类
            //foreach (var item in topMenus)
            //{
            //    List<HomeMenuInfoDTO> childMenus = allMenus.Where(it=>it.ParentId==item.Id).Select(it => new HomeMenuInfoDTO()
            //    {
            //        Id = it.Id,
            //        PId = it.ParentId,
            //        Title = it.Title,
            //        Href = it.Href,
            //        Icon = it.Icon
            //    }).ToList();

            //    //把子类添加到Child属性
            //    item.Child = childMenus;
            //}
            #endregion


            return topMenus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentMenus"></param>
        /// <param name="allMenus"></param>
        public void GetMenuChild(List<HomeMenuInfoDTO> parentMenus,List<MenuInfo> allMenus)
        {
            foreach (var item in parentMenus)
            {
                List<HomeMenuInfoDTO> childMenus = allMenus.Where(it => it.ParentId == item.Id).Select(it => new HomeMenuInfoDTO()
                {
                    Id = it.Id,
                    PId = it.ParentId,
                    Title = it.Title,
                    Href = it.Href,
                    Icon = it.Icon
                }).ToList();

                GetMenuChild(childMenus, allMenus);

                //把子类添加到Child属性
                item.Child = childMenus;
            }
        }
        #endregion
    }
}
