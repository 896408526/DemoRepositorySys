using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class R_UserInfo_RoleInfoDAL : BaseDAL<R_UserInfo_RoleInfo>,IR_UserInfo_RoleInfoDAL
    {
        private RepositorySysDBcontext _dbContext;
        public R_UserInfo_RoleInfoDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
