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
	public class CategoryBLL : ICategoryBLL
    {
        private ICategoryDAL _categoryDAL;
        private RepositorySysDBcontext _dbContext;

        public CategoryBLL(
            ICategoryDAL categoryDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _categoryDAL = categoryDAL;
            _dbContext = dbContext;
        }

        #region 获取耗材表的方法 (GetCategoryes)
        /// <summary>
        /// 获取耗材表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="roleName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetCategoryDTO> GetCategoryes(int limit, int page, string categoryName, out int count)
        {
            var data = _categoryDAL.GetEntities()
                       .Where(r => r.IsDelete == false)
                       .Select(r => new GetCategoryDTO
                       {
                           Id = r.Id,
                           CategoryName = r.CategoryName,
                           Description = r.Description,
                           CreateTime = r.CreateTime,
                           IsDelete = r.IsDelete
                       });

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                //条件精准查询
                data = data.Where(d => d.CategoryName == categoryName);
            }
            //计算数据总数
            count = data.Count();
            //分页(降序)
            return data.OrderByDescending(it => it.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();
        }
        #endregion

        #region 创建耗材 (CreateCategory)
        /// <summary>
        /// 创建耗材
        /// </summary>
        /// <param name="cate">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool CreateCategory(Category cate, out string msg)
        {
            if (string.IsNullOrWhiteSpace(cate.CategoryName))
            {
                msg = "名称不能为空";
                return false;
            }
            //验证部门是否存在
            if (_categoryDAL.GetEntities().FirstOrDefault(it => it.CategoryName == cate.CategoryName) != null)
            {
                msg = "名称已经存在";
                return false;
            }
            cate.Id = Guid.NewGuid().ToString();//用户id
            cate.CreateTime = DateTime.Now;//设置时间
            bool IsSuccess = _categoryDAL.CreateEntity(cate);//调用方法
            msg = IsSuccess ? $"添加{cate.CategoryName}成功" : "添加用户失败";

            return IsSuccess;
        }
        #endregion

        #region 删除的方法 (DeleteCategory)
        /// <summary>
        /// 删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteCategory(string id)
        {
            Category cate = _categoryDAL.GetEntityById(id);
            if (cate == null)
            {
                return false;
            }
            cate.IsDelete = true;
            cate.DeleteTime = DateTime.Now;
            _categoryDAL.UpdateEntity(cate);
            return true;
        }
        #endregion

        #region 批量删除的方法 (DeleteListCategory)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteListCategory(List<string> ids)
        {
            List<Category> cateList = _categoryDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                Category cate = _categoryDAL.GetEntityById(item);
                if (cateList == null)
                {
                    return false;
                }
                cate.IsDelete = true;
                cate.DeleteTime = DateTime.Now;

                _categoryDAL.UpdateEntity(cate);
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
        public bool UpdateCategory(Category cate, out string msg)
        {
            if (string.IsNullOrWhiteSpace(cate.CategoryName))
            {
                msg = $"部门名称不能为空";
                return false;
            }
            Category entity = _categoryDAL.GetEntityById(cate.Id);
            if (entity.Id == null)
            {
                msg = "ID不存在";
                return false;
            }
            //判断重复
            if (entity.CategoryName != cate.CategoryName)
            {
                var data = _categoryDAL.GetEntities().FirstOrDefault(it => it.CategoryName == cate.CategoryName);
                if (data != null)
                {
                    msg = "部门名已经被占用";
                    return false;
                }
            }

            entity.CategoryName = cate.CategoryName;
            entity.Description = cate.Description;

            bool IsSuccess = _categoryDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改用户失败";

            return IsSuccess;
        }
        #endregion
    }
}
