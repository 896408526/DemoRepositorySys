using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class WorkFlow_InstanceDAL : BaseDAL<WorkFlow_Instance>, IWorkFlow_InstanceDAL
    {
        private RepositorySysDBcontext _dbContext;
        public WorkFlow_InstanceDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
