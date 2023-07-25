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
    public class MenuInfoDAL : BaseDeleteDAL<MenuInfo>, IMenuInfoDAL
    {
        private RepositorySysDBcontext _dbContext;

        public MenuInfoDAL(
            RepositorySysDBcontext dbContext
            ):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
