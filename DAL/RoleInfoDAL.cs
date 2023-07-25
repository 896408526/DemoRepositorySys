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
    public class RoleInfoDAL : BaseDeleteDAL<RoleInfo>,IRoleInfoDAL
    {
        //实例化数据库上下文
        private RepositorySysDBcontext _dbContext;

        public RoleInfoDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
