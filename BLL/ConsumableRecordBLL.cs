using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model;
using Model.DTO;

namespace BLL
{
    public class ConsumableRecordBLL : IConsumableRecordBLL
    {
        private IConsumableRecordDAL _consumableRecordDAL;
        private RepositorySysDBcontext _dbContext;

        public ConsumableRecordBLL(
            IConsumableRecordDAL consumableRecordDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _consumableRecordDAL = consumableRecordDAL;
            _dbContext = dbContext;
        }

        #region 获取耗材记录表的方法 (GetConsumableRecordes)
        /// <summary>
        /// 获取耗材记录表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetConsumableRecordDTO> GetConsumableRecordes(int page, int limit, out int count)
        {
            var data = from d in _dbContext.ConsumableRecord
                       join c in _dbContext.ConsumableInfo.Where(d => d.IsDelete == false)
                       on d.ConsumableId equals c.Id
                       into DCtemp
                       from dc in DCtemp.DefaultIfEmpty()
                       join u in _dbContext.UserInfo.Where(d => d.IsDelete == false)
                       on d.Creator equals u.Id
                       into DUtemp
                       from du in DUtemp.DefaultIfEmpty()
                       select new GetConsumableRecordDTO
                       {
                           Id = d.Id,
                           ConsumableId = dc.ConsumableName,
                           Num = d.Num,
                           Type = d.Type,
                           Creator = du.UserName,
                           CreateTime = d.CreateTime
                       };

            //计算数据总数
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderByDescending(u => u.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 批量删除的方法 (DeleteListConsumableRecord)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteListConsumableRecord(List<string> ids)
        {
            List<ConsumableRecord> consList = _consumableRecordDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                ConsumableRecord cons = _consumableRecordDAL.GetEntityById(item);
                if (consList == null)
                {
                    return false;
                }
                _consumableRecordDAL.DeleteEntity(cons);
            }
            return true;
        }
        #endregion
    }
}
