using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DepartmentInfoDAL : BaseDeleteDAL<DepartmentInfo>, IDepartmentInfoDAL
    {
        //实例化数据库上下文
        private RepositorySysDBcontext _dbContext;
        public DepartmentInfoDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
