using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class WorkFlow_ModelDAL : BaseDAL<WorkFlow_Model>, IWorkFlow_ModelDAL
    {
        private RepositorySysDBcontext _dbContext;
        public WorkFlow_ModelDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
