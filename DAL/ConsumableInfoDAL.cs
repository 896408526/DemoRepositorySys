using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ConsumableInfoDAL :  BaseDeleteDAL<ConsumableInfo>,IConsumableInfoDAL
    {
        private RepositorySysDBcontext _dbContext;
        public ConsumableInfoDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
