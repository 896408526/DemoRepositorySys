using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class R_RoleInfo_MenuInfoDAL : BaseDAL<R_RoleInfo_MenuInfo>, IR_RoleInfo_MenuInfoDAL
    {
        private RepositorySysDBcontext _dbContext;

        public R_RoleInfo_MenuInfoDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
