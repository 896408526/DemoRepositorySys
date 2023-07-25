using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;

namespace IDAL
{
    /// <summary>
    /// 添加实体方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //泛型T，如果返回的不是泛型T则返回所有类型的基类
    public interface IBaseDAL<T> where T:BaseEntity
    {
        /// <summary>
        /// 获取实体方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DbSet<T> GetEntities();

        /// <summary>
        /// 获取单个对象根据ID获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntityById(string id);

        /// <summary>
        /// 添加实体方法
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool CreateEntity(T entity);

        /// <summary>
        /// 修改实体方法
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <returns></returns>
        bool UpdateEntity(T entity);

        /// <summary>
        /// 删除实体方法根据ID
        /// </summary>
        /// <param name="id">根据ID</param>
        /// <returns></returns>
        bool DeleteEntityById(string id);

        /// <summary>
        /// 删除实体方法根据实体
        /// </summary>
        /// <param name="entity">根据实体</param>
        /// <returns></returns>
        bool DeleteEntity(T entity);
    }
}
