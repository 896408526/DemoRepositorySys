using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Model;

namespace DAL
{
    public class ConsumableRecordDAL : BaseDAL<ConsumableRecord>, IConsumableRecordDAL
    {
        private RepositorySysDBcontext _dbContext;
        public ConsumableRecordDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
