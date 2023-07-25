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
    public class UserInfoDAL : BaseDeleteDAL<UserInfo>,IUserInfoDAL
    {
        //实例化数据库上下文
        private RepositorySysDBcontext _dbContext;
        public UserInfoDAL (
            RepositorySysDBcontext dbContext
            ) : base(dbContext)
        {
            //_dbContext = new RepositorySysDBcontext();
            _dbContext = dbContext;
        }

        /// <summary>
        /// 用户表数据访问层接口
        /// </summary>
        public DbSet<UserInfo> GetUserInfos()
        {
            return _dbContext.UserInfo;//返回UserInfo表
        }

        //未封装(添加)使用的方法
        //public bool CreateUserInfos(UserInfo user)
        //{
        //    _dbContext.UserInfo.Add(user);
        //    return _dbContext.SaveChanges() > 0;
            
        //}
    }
}
