using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Model;

namespace DAL
{
	public class CategoryDAL : BaseDeleteDAL<Category>, ICategoryDAL
    {
        private RepositorySysDBcontext _dbContext;
        public CategoryDAL(
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
