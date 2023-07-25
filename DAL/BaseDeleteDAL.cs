using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using IDAL;

namespace DAL
{
    public class BaseDeleteDAL<T> :BaseDAL<T>,IBaseDeleteDAL<T> where T : BaseDeleteEntity 
    {
        RepositorySysDBcontext _dbContext;

        public BaseDeleteDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">根据ID</param>
        /// <returns></returns>
        public T GetEntityById(string id)
        {
            return _dbContext.Set<T>().FirstOrDefault(it => it.Id == id);
        }
    }
}
