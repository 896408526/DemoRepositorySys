using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class WorkFlow_InstanceStepDAL : BaseDAL<WorkFlow_InstanceStep>, IWorkFlow_InstanceStepDAL
    {
        private RepositorySysDBcontext _dbContext;
        public WorkFlow_InstanceStepDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
