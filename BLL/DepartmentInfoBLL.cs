using IBLL;
using IDAL;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DepartmentInfoBLL : IDepartmentInfoBLL
    {
        private IUserInfoDAL _userInfoDAL;
        private IDepartmentInfoDAL _departmentInfoDAL;
        private RepositorySysDBcontext _dbContext;

        public DepartmentInfoBLL(
            IUserInfoDAL userInfoDAL
            , IDepartmentInfoDAL departmentInfoDAL
            , RepositorySysDBcontext dbContext
            )
        {
            _userInfoDAL = userInfoDAL;
            _departmentInfoDAL = departmentInfoDAL;
            _dbContext = dbContext;
        }

        #region 获取部门表的方法 (GetDepartmentInfos)
        /// <summary>
        /// 获取部门表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="departmentName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetDepartmentInfoDTO> GetDepartmentInfos(int page, int limit, string departmentName, out int count)
        {
            var data = from d in _dbContext.DepartmentInfo.Where(d => d.IsDelete == false)
                       join u in _dbContext.UserInfo.Where(u => u.IsDelete == false)
                       on d.LeaderId equals u.Id
                       into tempUD
                       from dd in tempUD.DefaultIfEmpty()
                       //再链父级部门
                       join d2 in _dbContext.DepartmentInfo.Where(u => u.IsDelete == false)
                       on d.ParentId equals d2.Id
                       into TempUD
                       from dd2 in TempUD.DefaultIfEmpty()
                       select new GetDepartmentInfoDTO
                       {
                           Id = d.Id,
                           DepartmentName = d.DepartmentName,
                           LeaderId = dd.UserName,
                           ParentId = dd2.DepartmentName,
                           Description = d.Description,
                           CreateTime = d.CreateTime
                       };

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                //条件精准查询
                data = data.Where(d => d.DepartmentName == departmentName);
            }
            //计算数据总数
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderByDescending(u => u.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 创建部门 (CreateDepartmentInfo)
        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="dept">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool CreateDepartmentInfo(DepartmentInfo dept, out string msg)
        {
            if (string.IsNullOrWhiteSpace(dept.DepartmentName))
            {
                msg = "部门名称不能为空";
                return false;
            }
            //验证部门是否存在
            if (_departmentInfoDAL.GetEntities().FirstOrDefault(it => it.DepartmentName == dept.DepartmentName) != null)
            {
                msg = "部门已经存在";
                return false;
            }
            dept.Id = Guid.NewGuid().ToString();//用户id
            dept.CreateTime = DateTime.Now;//设置时间
            bool IsSuccess = _departmentInfoDAL.CreateEntity(dept);//调用方法
            msg = IsSuccess ? $"添加{dept.DepartmentName}成功" : "添加用户失败";
            
            return IsSuccess;
        }
        #endregion

        #region 删除的方法 (DeleteDepartmentInfo)
        /// <summary>
        /// 删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDepartmentInfo(string id)
        {
            DepartmentInfo dept = _departmentInfoDAL.GetEntityById(id);
            if (dept == null)
            {
                return false;
            }
            dept.IsDelete = true;
            dept.DeleteTime = DateTime.Now;
            _departmentInfoDAL.UpdateEntity(dept);
            return true;
        }
        #endregion

        #region 批量删除的方法 (DeleteListDepartmentInfo)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteListDepartmentInfo(List<string> ids)
        {
            List<DepartmentInfo> deptList = _departmentInfoDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                DepartmentInfo dept = _departmentInfoDAL.GetEntityById(item);
                if (deptList == null)
                {
                    return false;
                }
                dept.IsDelete = true;
                dept.DeleteTime = DateTime.Now;

                _departmentInfoDAL.UpdateEntity(dept);
            }
            return true;
        }
        #endregion

        #region 修改的方法 (UpdateDepartmentInfo)
        /// <summary>
        /// 修改用户的方法
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateDepartmentInfo(DepartmentInfo dept, out string msg)
        {
            if (string.IsNullOrWhiteSpace(dept.DepartmentName))
            {
                msg = $"部门名称不能为空";
                return false;
            }
            DepartmentInfo entity = _departmentInfoDAL.GetEntityById(dept.Id);
            if (entity.Id == null)
            {
                msg = "ID不存在";
                return false;
            }
            //判断重复
            if(entity.DepartmentName!= dept.DepartmentName)
            {
                var data = _departmentInfoDAL.GetEntities().FirstOrDefault(it => it.DepartmentName == dept.DepartmentName);
                if (data != null)
                {
                    msg = "部门名已经被占用";
                    return false;
                }
            } 

            entity.DepartmentName = dept.DepartmentName;
            entity.LeaderId = dept.LeaderId;
            entity.ParentId = dept.ParentId;
            entity.Description = dept.Description;

            bool IsSuccess = _departmentInfoDAL.UpdateEntity(entity);//调用方法

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
        public DepartmentInfo GetDepartmentInfoById(string id)
        {
            return _departmentInfoDAL.GetEntityById(id);
        }
        #endregion

        #region 获取数据库列表赋值下拉框 (GetSelectOption)
        /// <summary>
        /// 获取主管和父级部门下拉框的方法
        /// </summary>
        /// <returns></returns>
        public object GetSelectOption()
        {
            var parentData = _dbContext.DepartmentInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.DepartmentName,
            });

            var leaderData = _dbContext.UserInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.UserName,
            });

            return new
            {
                parentData = parentData,
                leaderData = leaderData
            };
        }
        #endregion
    }
}
