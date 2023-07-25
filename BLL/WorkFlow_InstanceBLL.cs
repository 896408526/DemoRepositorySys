using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model;
using Model.DTO;
using Model.Enums;

namespace BLL
{
    public class WorkFlow_InstanceBLL : IWorkFlow_InstanceBLL
    {
        #region 构造
        private IWorkFlow_InstanceDAL _workFlow_InstanceDAL;
        private RepositorySysDBcontext _dbContext;

        public WorkFlow_InstanceBLL(
            IWorkFlow_InstanceDAL workFlow_InstanceDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _workFlow_InstanceDAL = workFlow_InstanceDAL;
            _dbContext = dbContext;
        }
        #endregion

        #region 获取工作流实例的方法 (GetWorkFlow_Instances)
        /// <summary>
        /// 获取工作流实例的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetWorkFlow_InstanceDTO> GetWorkFlow_Instances(int page, int limit, string userId, out int count)
        {
            var data = from i in _dbContext.WorkFlow_Instance.Where(it => it.Creator == userId)
                       join m in _dbContext.WorkFlow_Model
                       on i.ModelId equals m.Id
                       into IMtemp
                       from im in IMtemp.DefaultIfEmpty()

                       join c in _dbContext.ConsumableInfo
                       on i.OutGoodsId equals c.Id
                       into ICtemp
                       from ic in ICtemp.DefaultIfEmpty()

                       join u in _dbContext.UserInfo
                       on i.Creator equals u.Id
                       into IUtemp
                       from iu in IUtemp.DefaultIfEmpty()
                       select new GetWorkFlow_InstanceDTO
                       {
                           Id = i.Id,
                           Status = i.Status,
                           Description = i.Description,
                           Reason = i.Reason,
                           OutNum = i.OutNum,
                           CreateTime = i.CreateTime,
                           Creator = iu.UserName,
                           OutGoodsId = ic.ConsumableName,
                           ModelId = im.Title,
                       };

            //计算数据总数
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderByDescending(u => u.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 创建工作流实例 (CreateWorkFlow_Instance)
        /// <summary>
        /// 创建工作流实例
        /// </summary>
        /// <param name="param">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool CreateWorkFlow_Instance(WorkFlow_Instance param,string userId ,out string msg)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    WorkFlow_Instance workFlow_Instance = new WorkFlow_Instance()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreateTime = DateTime.Now,
                        Creator = userId,
                        Description = param.Description,
                        ModelId = param.ModelId,
                        OutGoodsId = param.OutGoodsId,
                        OutNum = param.OutNum,
                        Reason = param.Reason,
                        Status = (int)WorkFlow_InstanceStatusEnums.审核中,
                    };
                    _dbContext.WorkFlow_Instance.Add(workFlow_Instance);

                    bool b = _dbContext.SaveChanges() > 0;
                    if (b == false)
                    {
                        transaction.Rollback();
                        msg = "发起申领失败";
                        return false;
                    }

                    UserInfo user = _dbContext.UserInfo.FirstOrDefault(it => it.Id == userId);
                    if (user == null || string.IsNullOrWhiteSpace(user.DepartmentId))
                    {
                        transaction.Rollback();
                        msg = "当前用户没有部门";
                        return false;
                    }

                    DepartmentInfo dept = _dbContext.DepartmentInfo.FirstOrDefault(it => it.Id == user.DepartmentId);
                    if (dept == null || dept.LeaderId == null) 
                    {
                        transaction.Rollback();
                        msg = "当前部门未设置领导";
                        return false;
                    }

                    int count = (from ur in _dbContext.R_UserInfo_RoleInfo.Where(it => it.UserId == dept.LeaderId)
                                join r in _dbContext.RoleInfo.Where(it => it.RoleName == "部门经理")
                                on ur.RoleId equals r.Id
                                select r.RoleName).Count();

                    if (count <= 0)
                    {
                        transaction.Rollback();
                        msg = "当前用户权限不足";
                        return false;
                    }

                    WorkFlow_InstanceStep flow_InstanceStep = new WorkFlow_InstanceStep()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreateTime = DateTime.Now,
                        InstanceId = workFlow_Instance.Id,
                        ReviewerId = dept.LeaderId,
                        ReviewStatus = (int)WorkFlow_InstanceStepStatusEnums.审核中,
                    };
                    _dbContext.WorkFlow_InstanceStep.Add(flow_InstanceStep);

                    bool b2 = _dbContext.SaveChanges() > 0;
                    if (b2 == false)
                    {
                        transaction.Rollback();
                        msg = "发起申领失败";
                        return false;
                    }

                    msg = "耗材申领发起成功";
                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    msg = ex.Message;
                    return false;
                }


            }
        }
        #endregion

        #region 修改的方法 (UpdateWorkFlow_Instance)
        /// <summary>
        /// 修改的方法
        /// </summary>
        /// <param name="workisce"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateWorkFlow_Instance(WorkFlow_Instance workisce, out string msg)
        {
            WorkFlow_Instance entity = _workFlow_InstanceDAL.GetEntityById(workisce.Id);

            entity.ModelId = workisce.ModelId;
            entity.Status = workisce.Status;
            entity.Description = workisce.Description;
            entity.Reason = workisce.Reason;
            entity.Creator = workisce.Creator;
            entity.OutNum = workisce.OutNum;
            entity.OutGoodsId = workisce.OutGoodsId;

            bool IsSuccess = _workFlow_InstanceDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改用户失败";

            return IsSuccess;
        }
        #endregion

        #region 根据ID获取数据返回赋值 (GetWorkFlow_InstanceById)
        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WorkFlow_Instance GetWorkFlow_InstanceById(string id)
        {
            return _workFlow_InstanceDAL.GetEntityById(id);
        }
        #endregion

        #region 获取数据库列表赋值下拉框 (GetSelectOption)
        /// <summary>
        /// 获取数据库列表赋值下拉框
        /// </summary>
        /// <returns></returns>
        public object GetSelectOption()
        {
            var goodsData = _dbContext.ConsumableInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.ConsumableName,
            });

            var userData = _dbContext.UserInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.UserName,
            });

            var modelData = _dbContext.WorkFlow_Model.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.Title,
            });

            return new
            {
                goodsData = goodsData,
                userData = userData,
                modelData = modelData
            };
        }
        #endregion
    }
}
