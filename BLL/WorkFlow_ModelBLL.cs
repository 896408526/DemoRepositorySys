using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model;
using Model.DTO;

namespace BLL
{
    public class WorkFlow_ModelBLL : IWorkFlow_ModelBLL
    {
        private IWorkFlow_ModelDAL _workFlow_ModelDAL;
        private RepositorySysDBcontext _dbContext;

        public WorkFlow_ModelBLL(
            IWorkFlow_ModelDAL workFlow_ModelDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _workFlow_ModelDAL = workFlow_ModelDAL;
            _dbContext = dbContext;
        }

        #region 获取工作模板表的方法 (GetWorkFlow_Modeles)
        /// <summary>
        /// 获取工作模板表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="title"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetWorkFlow_ModelDTO> GetWorkFlow_Modeles(int limit, int page, string title, out int count)
        {
            var data = _workFlow_ModelDAL.GetEntities()
                       .Where(r => r.IsDelete == false)
                       .Select(r => new GetWorkFlow_ModelDTO
                       {
                           Id = r.Id,
                           Title = r.Title,
                           Description = r.Description,
                           CreateTime = r.CreateTime,
                       });

            if (!string.IsNullOrWhiteSpace(title))
            {
                data = data.Where(d => d.Title.Contains(title));
            }
            //计算数据总数
            count = data.Count();
            //分页(降序)
            return data.OrderByDescending(it => it.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();
        }
        #endregion

        #region 创建工作模板 (CreateWorkFlow_Model)
        /// <summary>
        /// 创建工作模板
        /// </summary>
        /// <param name="cate">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool CreateWorkFlow_Model(WorkFlow_Model workmode, out string msg)
        {
            if (string.IsNullOrWhiteSpace(workmode.Title))
            {
                msg = "名称不能为空";
                return false;
            }
            //验证部门是否存在
            if (_workFlow_ModelDAL.GetEntities().FirstOrDefault(it => it.Title == workmode.Title) != null)
            {
                msg = "名称已经存在";
                return false;
            }
            workmode.Id = Guid.NewGuid().ToString();//用户id
            workmode.CreateTime = DateTime.Now;//设置时间
            bool IsSuccess = _workFlow_ModelDAL.CreateEntity(workmode);//调用方法
            msg = IsSuccess ? $"添加{workmode.Title}成功" : "添加模板失败";

            return IsSuccess;
        }
        #endregion

        #region 删除的方法 (DeleteWorkFlow_Model)
        /// <summary>
        /// 删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteWorkFlow_Model(string id)
        {
            WorkFlow_Model cate = _workFlow_ModelDAL.GetEntityById(id);
            if (cate == null)
            {
                return false;
            }
            cate.IsDelete = true;
            cate.DeleteTime = DateTime.Now;
            _workFlow_ModelDAL.UpdateEntity(cate);
            return true;
        }
        #endregion

        #region 批量删除的方法 (DeleteListWorkFlow_Model)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteListWorkFlow_Model(List<string> ids)
        {
            List<WorkFlow_Model> cateList = _workFlow_ModelDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                WorkFlow_Model cate = _workFlow_ModelDAL.GetEntityById(item);
                if (cateList == null)
                {
                    return false;
                }
                cate.IsDelete = true;
                cate.DeleteTime = DateTime.Now;

                _workFlow_ModelDAL.UpdateEntity(cate);
            }
            return true;
        }
        #endregion

        #region 修改的方法 (UpdateCategory)
        /// <summary>
        /// 修改用户的方法
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateWorkFlow_Model(WorkFlow_Model workmode, out string msg)
        {
            if (string.IsNullOrWhiteSpace(workmode.Title))
            {
                msg = $"标题名称不能为空";
                return false;
            }
            WorkFlow_Model entity = _workFlow_ModelDAL.GetEntityById(workmode.Id);
            if (entity.Id == null)
            {
                msg = "ID不存在";
                return false;
            }
            //判断重复
            if (entity.Title != workmode.Title)
            {
                var data = _workFlow_ModelDAL.GetEntities().FirstOrDefault(it => it.Title == workmode.Title);
                if (data != null)
                {
                    msg = "标题名已经被占用";
                    return false;
                }
            }

            entity.Title = workmode.Title;
            entity.Description = workmode.Description;

            bool IsSuccess = _workFlow_ModelDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改模板失败";

            return IsSuccess;
        }
        #endregion
    }
}
