using IBLL;
using IDAL;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Model.Enums;


namespace BLL
{
    public class WorkFlow_InstanceStepBLL : IWorkFlow_InstanceStepBLL
    {
        #region 构造
        private IWorkFlow_InstanceStepDAL _workFlow_InstanceStepDAL;
        private IWorkFlow_InstanceDAL _workFlow_InstanceDAL;
        private RepositorySysDBcontext _dbContext;

        public WorkFlow_InstanceStepBLL(
            IWorkFlow_InstanceStepDAL workFlow_InstanceStepDAL,
            IWorkFlow_InstanceDAL workFlow_InstanceDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _workFlow_InstanceStepDAL = workFlow_InstanceStepDAL;
            _workFlow_InstanceDAL = workFlow_InstanceDAL;
            _dbContext = dbContext;
        }
        #endregion

        #region 获取工作流实例的方法 (GetWorkFlow_InstanceStepes)
        /// <summary>
        /// 获取工作流实例的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetWorkFlow_InstanceStepDTO> GetWorkFlow_InstanceStepes(int page, int limit,string userId ,out int count)
        {
            var data = from ws in _dbContext.WorkFlow_InstanceStep.Where(it => it.ReviewerId == userId)
                       join w in _dbContext.WorkFlow_Instance
                       on ws.InstanceId equals w.Id
                       into WWtemp
                       from ww in WWtemp.DefaultIfEmpty()

                       join c in _dbContext.ConsumableInfo
                       on ww.OutGoodsId equals c.Id
                       into WWCtemp
                       from wwc in WWCtemp.DefaultIfEmpty()

                       join u in _dbContext.UserInfo
                       on ww.Creator equals u.Id
                       into WWUtemp
                       from wwu in WWUtemp.DefaultIfEmpty()

                       join u2 in _dbContext.UserInfo
                       on ws.ReviewerId equals u2.Id
                       into WWU2temp
                       from wwu2 in WWU2temp.DefaultIfEmpty()
                       select new GetWorkFlow_InstanceStepDTO
                       {
                           Id = ws.Id,
                           InstanceId = ws.InstanceId,
                           ReviewerId = ws.ReviewerId,  
                           ReviewReason = ws.ReviewReason,
                           ReviewStatus = ws.ReviewStatus,
                           ReviewTime = ws.ReviewTime,
                           BeforeStepId = ws.BeforeStepId,
                           CreateTime = ws.CreateTime,

                           CreatorId = ww.Creator,
                           CreatorName = wwu.UserName,
                           OutNum = ww.OutNum,
                           OutGoodsName = wwc.ConsumableName,
                           Reason = ww.Reason,
                           Creator = wwu2.UserName,
                       };

            //计算数据总数
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderByDescending(u => u.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 审核方法 (UpdateWorkFlow_InstanceStep)
        /// <summary>
        /// 审核方法
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateWorkFlow_InstanceStep(WorkFlow_InstanceStep workStep, string userId, int outNum, out string msg)
        {
            using (var transation = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    #region 先审核自己的
                    WorkFlow_InstanceStep workFlow_InstanceStep = _dbContext.WorkFlow_InstanceStep.FirstOrDefault(it => it.Id == workStep.Id);
                    if (workFlow_InstanceStep == null)
                    {
                        transation.Rollback();
                        msg = "未找到该工作流步骤";
                        return false;
                    }
                    if (workFlow_InstanceStep.ReviewerId != userId)
                    {
                        transation.Rollback();
                        msg = "权限不足";
                        return false;
                    }
                    workFlow_InstanceStep.ReviewReason = workStep.ReviewReason;
                    workFlow_InstanceStep.ReviewStatus = workStep.ReviewStatus;
                    workFlow_InstanceStep.ReviewTime = DateTime.Now;

                    _dbContext.Entry(workFlow_InstanceStep).State = EntityState.Modified;
                    bool IsSuccess = _dbContext.SaveChanges() > 0;//调用方法
                    if (IsSuccess == false)
                    {
                        transation.Rollback();
                        msg = "审核失败";
                        return false;
                    }
                    #endregion

                    //获取当前用户的多个角色
                    List<string> roleName = (
                                             from ur in _dbContext.R_UserInfo_RoleInfo.Where(it => it.UserId == userId)
                                             join r in _dbContext.RoleInfo
                                             on ur.RoleId equals r.Id
                                             select r.RoleName
                                             ).ToList();

                    if (roleName.Contains("部门经理"))
                    {
                        if (workStep.ReviewStatus == (int)WorkFlow_InstanceStepStatusEnums.同意)
                        {
                            //获取当前角色找多个仓库管理员的id集
                            List<string> cangUserId = (
                                             from r in _dbContext.RoleInfo.Where(it => it.RoleName == "仓库管理员" && !it.IsDelete)
                                             join ur in _dbContext.R_UserInfo_RoleInfo
                                             on r.Id equals ur.RoleId
                                             select ur.UserId
                                             ).ToList();

                            string ckUserId = cangUserId.FirstOrDefault();
                            UserInfo ckUserInfo = _dbContext.UserInfo.FirstOrDefault(it => it.Id == ckUserId);
                            if (ckUserInfo == null)
                            {
                                transation.Rollback();
                                msg = "审核时创建步骤失败";
                                return false;
                            }

                            WorkFlow_InstanceStep entity = new WorkFlow_InstanceStep()
                            {
                                Id = Guid.NewGuid().ToString(),
                                BeforeStepId = workFlow_InstanceStep.BeforeStepId,
                                InstanceId = workFlow_InstanceStep.InstanceId,
                                CreateTime = DateTime.Now,
                                ReviewerId = ckUserInfo.Id,
                                ReviewStatus = (int)WorkFlow_InstanceStepStatusEnums.审核中,
                            };
                            _dbContext.WorkFlow_InstanceStep.Add(entity);
                            IsSuccess = _dbContext.SaveChanges() > 0;//调用方法
                            if (IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "审核失败";
                                return false;
                            }

                        }
                        else if (workStep.ReviewStatus == (int)WorkFlow_InstanceStepStatusEnums.驳回)
                        {
                            WorkFlow_Instance workFlow_Instance = _dbContext.WorkFlow_Instance.FirstOrDefault(it => it.Id == workFlow_InstanceStep.InstanceId);
                            if (workFlow_Instance == null)
                            {
                                transation.Rollback();
                                msg = "未找到该工作流步骤";
                                return false;
                            }

                            WorkFlow_InstanceStep entity = new WorkFlow_InstanceStep()
                            {
                                Id = Guid.NewGuid().ToString(),
                                BeforeStepId = workFlow_InstanceStep.Id,
                                InstanceId = workFlow_InstanceStep.InstanceId,
                                CreateTime = DateTime.Now,
                                ReviewerId = workFlow_Instance.Creator,
                                ReviewStatus = (int)WorkFlow_InstanceStepStatusEnums.审核中,
                            };
                            _dbContext.WorkFlow_InstanceStep.Add(entity);

                            IsSuccess = _dbContext.SaveChanges() > 0;//调用方法
                            if (IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "审核时创建步骤失败";
                                return false;
                            }
                        }
                        else
                        {
                            transation.Rollback();
                            msg = "操作错误";
                            return false;
                        }
                    }
                    else if (roleName.Contains("仓库管理员")){
                        if(workFlow_InstanceStep.ReviewStatus == (int)WorkFlow_InstanceStepStatusEnums.同意)
                        {
                            WorkFlow_Instance workFlow_Instance = _dbContext.WorkFlow_Instance.FirstOrDefault(it => it.Id == workFlow_InstanceStep.InstanceId);
                            if (workFlow_Instance == null)
                            {
                                transation.Rollback();
                                msg = "未找到该工作流步骤";
                                return false;
                            }
                            workFlow_Instance.Status = (int)WorkFlow_InstanceStatusEnums.结束;

                            _dbContext.Entry(workFlow_Instance).State = EntityState.Modified;
                            IsSuccess = _dbContext.SaveChanges() > 0;
                            if (IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "结束工作流失败";
                                return false;
                            }

                            ConsumableInfo consumableInfo = _dbContext.ConsumableInfo.FirstOrDefault(it => it.Id == workFlow_Instance.OutGoodsId);
                            if (consumableInfo == null)
                            {
                                transation.Rollback();
                                msg = "耗材不存在";
                                return false;
                            }
                            if (consumableInfo.Num - workFlow_Instance.OutNum < 0)
                            {
                                transation.Rollback();
                                msg = "库存不能小于零";
                                return false;
                            }

                            consumableInfo.Num -= workFlow_Instance.OutNum;

                            _dbContext.Entry(consumableInfo).State = EntityState.Modified;
                            IsSuccess = _dbContext.SaveChanges() > 0;
                            if (IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "创建工作流失败";
                                return false;
                            }

                            ConsumableRecord consumableRecord = new ConsumableRecord()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Num = workFlow_Instance.OutNum,
                                CreateTime = DateTime.Now,
                                Type = (int)ConsumableRecordTypeEnums.出库,
                                Creator = userId,
                                ConsumableId = consumableInfo.Id,
                            };
                            _dbContext.ConsumableRecord.Add(consumableRecord);
                            IsSuccess = _dbContext.SaveChanges() > 0;
                            if (IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "创建工作流失败";
                                return false;
                            }
                        }
                        else if (workFlow_InstanceStep.ReviewStatus == (int)WorkFlow_InstanceStepStatusEnums.驳回)
                        {
                            WorkFlow_InstanceStep newWorkFlow_InstanceStep = _dbContext.WorkFlow_InstanceStep.FirstOrDefault(it => it.Id == workFlow_InstanceStep.BeforeStepId);
                            if(newWorkFlow_InstanceStep == null)
                            {
                                transation.Rollback();
                                msg = "创建工作流失败";
                                return false;
                            }

                            WorkFlow_InstanceStep BeforeWorkFlow_InstanceStep = new WorkFlow_InstanceStep()
                            {
                                Id = Guid.NewGuid().ToString(),
                                BeforeStepId = workFlow_InstanceStep.Id,
                                CreateTime = DateTime.Now,
                                InstanceId = workFlow_InstanceStep.InstanceId,
                                ReviewerId = newWorkFlow_InstanceStep.ReviewerId,
                                ReviewStatus = (int)WorkFlow_InstanceStepStatusEnums.审核中,   
                            };
                            _dbContext.WorkFlow_InstanceStep.Add(BeforeWorkFlow_InstanceStep);
                            IsSuccess = _dbContext.SaveChanges()>0;
                            if(IsSuccess == false)
                            {
                                transation.Rollback();
                                msg = "驳回失败";
                                return false;
                            }
                        }
                        else
                        {
                            transation.Rollback();
                            msg = "审核状态错误";
                            return false;
                        }
                        
                    }
                    else if (roleName.Contains("普通职员")){
                        WorkFlow_Instance workFlow_Instance = _dbContext.WorkFlow_Instance.FirstOrDefault(it => it.Id == workFlow_InstanceStep.InstanceId);
                        if (workFlow_Instance == null)
                        {
                            transation.Rollback();
                            msg = "未找到该工作流步骤";
                            return false;
                        }
                        workFlow_Instance.OutNum = outNum;

                        _dbContext.Entry(workFlow_Instance).State = EntityState.Modified;
                        IsSuccess = _dbContext.SaveChanges() > 0;
                        if(IsSuccess == false)
                        {
                            transation.Rollback();
                            msg = "修改审核数量失败";
                            return false;
                        }

                        WorkFlow_InstanceStep oldWorkStep = _dbContext.WorkFlow_InstanceStep.FirstOrDefault(it => it.Id == workFlow_InstanceStep.BeforeStepId);
                        if (oldWorkStep == null)
                        {
                            transation.Rollback();
                            msg = "上一个步骤未找到";
                            return false;
                        }

                        WorkFlow_InstanceStep newWorkStep = new WorkFlow_InstanceStep()
                        {
                            Id = Guid.NewGuid().ToString(),
                            BeforeStepId = workFlow_InstanceStep.BeforeStepId,
                            CreateTime = DateTime.Now,
                            InstanceId = workFlow_Instance.Id,
                            ReviewerId = oldWorkStep.ReviewerId,
                            ReviewStatus = (int)WorkFlow_InstanceStepStatusEnums.审核中,
                        };
                        _dbContext.WorkFlow_InstanceStep.Add(newWorkStep);
                        IsSuccess = _dbContext.SaveChanges() > 0;
                        if (IsSuccess == false)
                        {
                            transation.Rollback();
                            msg = "上一个步骤未找到";
                            return false;
                        }
;                    }
                    else
                    {
                        transation.Rollback();
                        msg = "错误操作";
                        return false;
                    }
                    msg = "审核成功";
                    transation.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    msg = "审核失败" + ex.Message;
                    return false;
                }
            }
        }
        #endregion

        #region 根据ID获取数据返回赋值 (GetWorkFlow_InstanceStepById)
        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WorkFlow_InstanceStep GetWorkFlow_InstanceStepById(string id)
        {
            return _workFlow_InstanceStepDAL.GetEntityById(id);
        }
        #endregion

        #region 获取数据库列表赋值下拉框 (GetSelectOption)
        /// <summary>
        /// 获取数据库列表赋值下拉框
        /// </summary>
        /// <returns></returns>
        public object GetSelectOption()
        {
            var userData = _dbContext.UserInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.UserName,
            });

            var goodsData = _dbContext.ConsumableInfo.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.ConsumableName,
            });

            return new
            {
                goodsData = goodsData,
                userData = userData,
            };
        }
        #endregion
    }
}
