using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model;
using Model.DTO;
using Model.Enums;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data.Entity;

namespace BLL
{
    public class ConsumableInfoBLL : IConsumableInfoBLL
    {
        private IConsumableInfoDAL _consumableInfoDAL;
        private RepositorySysDBcontext _dbContext;

        public ConsumableInfoBLL(
            IConsumableInfoDAL consumableInfoDAL,
            RepositorySysDBcontext dbContext
            )
        {
            _consumableInfoDAL = consumableInfoDAL;
            _dbContext = dbContext;
        }

        #region 获取耗材信息表的方法 (GetConsumableInfoes)
        /// <summary>
        /// 获取耗材信息表的方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="consumableName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<GetConsumableInfoDTO> GetConsumableInfoes(int page, int limit, string consumableName, out int count)
        {
            var data = from d in _dbContext.ConsumableInfo.Where(d => d.IsDelete == false)
                       join c in _dbContext.Category.Where(d => d.IsDelete == false)
                       on d.CategoryId equals c.Id
                       into DCtemp
                       from dd in DCtemp.DefaultIfEmpty()
                       select new GetConsumableInfoDTO
                       {
                           Id = d.Id,
                           ConsumableName = d.ConsumableName,
                           CategoryId = dd.CategoryName,
                           Specification = d.Specification,
                           Num = d.Num,
                           Unit = d.Unit,
                           Money = d.Money,
                           Description = d.Description,
                           CreateTime = d.CreateTime
                       };

            if (!string.IsNullOrWhiteSpace(consumableName))
            {
                //条件精准查询
                data = data.Where(d => d.ConsumableName == consumableName);
            }
            //计算数据总数
            count = data.Count();
            //分页(降序)
            var listpage = data.OrderByDescending(u => u.CreateTime).Skip(limit * (page - 1)).Take(limit).ToList();

            return listpage;
        }
        #endregion

        #region 创建耗材信息 (CreateConsumableInfo)
        /// <summary>
        /// 创建耗材信息
        /// </summary>
        /// <param name="cate">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool CreateConsumableInfo(ConsumableInfo cons, out string msg)
        {
            if (string.IsNullOrWhiteSpace(cons.ConsumableName))
            {
                msg = "名称不能为空";
                return false;
            }
            //验证部门是否存在
            if (_consumableInfoDAL.GetEntities().FirstOrDefault(it => it.ConsumableName == cons.ConsumableName) != null)
            {
                msg = "名称已经存在";
                return false;
            }
            cons.Id = Guid.NewGuid().ToString();//用户id
            cons.CreateTime = DateTime.Now;//设置时间
            bool IsSuccess = _consumableInfoDAL.CreateEntity(cons);//调用方法
            msg = IsSuccess ? $"添加{cons.ConsumableName}成功" : "添加用户失败";

            return IsSuccess;
        }
        #endregion

        #region 删除的方法 (DeleteConsumableInfo)
        /// <summary>
        /// 删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteConsumableInfo(string id)
        {
            ConsumableInfo cons = _consumableInfoDAL.GetEntityById(id);
            if (cons == null)
            {
                return false;
            }
            cons.IsDelete = true;
            cons.DeleteTime = DateTime.Now;
            _consumableInfoDAL.UpdateEntity(cons);
            return true;
        }
        #endregion

        #region 批量删除的方法 (DeleteListConsumableInfo)
        /// <summary>
        /// 批量删除的方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteListConsumableInfo(List<string> ids)
        {
            List<ConsumableInfo> consList = _consumableInfoDAL.GetEntities().Where(it => ids.Contains(it.Id)).ToList();
            foreach (var item in ids)
            {
                ConsumableInfo cons = _consumableInfoDAL.GetEntityById(item);
                if (consList == null)
                {
                    return false;
                }
                cons.IsDelete = true;
                cons.DeleteTime = DateTime.Now;

                _consumableInfoDAL.UpdateEntity(cons);
            }
            return true;
        }
        #endregion

        #region 修改的方法 (UpdateConsumableInfo)
        /// <summary>
        /// 修改用户的方法
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateConsumableInfo(ConsumableInfo cons, out string msg)
        {
            if (string.IsNullOrWhiteSpace(cons.ConsumableName))
            {
                msg = $"名称不能为空";
                return false;
            }
            ConsumableInfo entity = _consumableInfoDAL.GetEntityById(cons.Id);
            if (entity.Id == null)
            {
                msg = "ID不存在";
                return false;
            }
            //判断重复
            if (entity.ConsumableName != cons.ConsumableName)
            {
                var data = _consumableInfoDAL.GetEntities().FirstOrDefault(it => it.ConsumableName == cons.ConsumableName);
                if (data != null)
                {
                    msg = "部门名已经被占用";
                    return false;
                }
            }

            entity.ConsumableName = cons.ConsumableName;
            entity.CategoryId = cons.CategoryId;
            entity.Specification = cons.Specification;
            entity.Num = cons.Num;
            entity.Unit = cons.Unit;
            entity.Money = cons.Money;
            entity.Description = cons.Description;

            bool IsSuccess = _consumableInfoDAL.UpdateEntity(entity);//调用方法

            msg = IsSuccess ? $"修改成功" : "修改用户失败";

            return IsSuccess;
        }
        #endregion

        #region 根据ID获取数据返回赋值 (GetMenuInfoById)
        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConsumableInfo GetConsumableInfoById(string id)
        {
            return _consumableInfoDAL.GetEntityById(id);
        }
        #endregion

        #region 获取数据库列表赋值下拉框 (GetSelectOption)
        /// <summary>
        /// 获取数据库列表赋值下拉框
        /// </summary>
        /// <returns></returns>
        public object GetSelectOption()
        {
            var consData = _dbContext.Category.Where(it => it.IsDelete == false).Select(d => new
            {
                value = d.Id,
                title = d.CategoryName,
            });

            return new
            {
                consData = consData
            };
        }
        #endregion

        #region Excel耗材入库方法 (UpLoad)
        /// <summary>
        /// Excel耗材入库方法
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extension">文件后缀</param>
        /// <param name="userId">用户ID</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        public bool UpLoad(Stream stream, string extension, string userId, out string msg)
        {
            IWorkbook wk = null;
            if (extension.Equals(".xls"))
            {
                wk = new HSSFWorkbook(stream);
            }
            else
            {
                wk = new XSSFWorkbook(stream);
            }

            stream.Close();
            stream.Dispose();

            //操作Excel表
            ISheet sheet = wk.GetSheetAt(0);//根据索引获取Excel表
            int rowNum = sheet.LastRowNum;//获取表有多少行数据

            //开启事务
            using (var transation = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 1; i <= rowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);//获取表中每行的数据

                        ICell cell = row.GetCell(0);//获取每行的第一列数据
                        string value = cell.ToString();

                        ICell cellNum = row.GetCell(2);
                        string StrNum = cellNum.ToString();

                        int IntNum;

                        bool par = int.TryParse(StrNum, out IntNum);//比较判断Num（数量）是否为Int类型
                        if (par == false)
                        {
                            transation.Rollback();//回滚事务
                            msg = $"第{i + 1}行耗材的实际购买数量有误";
                            return false;
                        }

                        ConsumableInfo consumableInfo = _consumableInfoDAL.GetEntities().FirstOrDefault(it =>it.IsDelete == false && it.ConsumableName == value);
                        if (consumableInfo == null)
                        {
                            transation.Rollback();//回滚事务
                            msg = $"第{i + 1}行耗材不存在或已被删除,{value}出现错误";
                            return false;
                        }

                        ConsumableRecord consumableRecord = new ConsumableRecord()
                        {
                            Id = Guid.NewGuid().ToString(),
                            ConsumableId = consumableInfo.Id,
                            CreateTime = DateTime.Now,
                            Creator = userId,
                            Num = IntNum,
                            Type = (int)ConsumableRecordTypeEnums.入库

                        };
                        _dbContext.ConsumableRecord.Add(consumableRecord);
                        bool IsOk = _dbContext.SaveChanges() > 0;
                        if (IsOk == false)
                        {
                            transation.Rollback();//回滚事务
                            msg = $"第{i + 1}行耗材记录失败,{value}出现错误";
                            return false;
                        }

                        consumableInfo.Num += IntNum;
                        _dbContext.Entry(consumableInfo).State = EntityState.Modified;
                        IsOk = _dbContext.SaveChanges() > 0;
                        if (IsOk == false)
                        {
                            transation.Rollback();//回滚事务
                            msg = $"第{i + 1}行耗材记录失败,{value}出现错误";
                            return false;
                        }

                    }

                    transation.Commit();
                    msg = "入库成功";
                    return true;
                }
                catch (Exception ex)
                {
                    transation.Rollback();
                    //msg = "入库错误";
                    msg = ex.Message;
                    return false;
                }
            }
        }
        #endregion

        #region 出入库记录的接口 (GetDownLoad)
        /// <summary>
        /// 出入库记录的接口
        /// </summary>
        /// <param name="downLoadName"></param>
        /// <returns></returns>
        public Stream GetDownLoad(out string downLoadName)
        {
            var datas = (from d in _dbContext.ConsumableRecord
                         join c in _dbContext.ConsumableInfo.Where(d => d.IsDelete == false)
                         on d.ConsumableId equals c.Id
                         into DCtemp
                         from dc in DCtemp.DefaultIfEmpty()
                         join u in _dbContext.UserInfo.Where(d => d.IsDelete == false)
                         on d.Creator equals u.Id
                         into DUtemp
                         from du in DUtemp.DefaultIfEmpty()
                         select new
                         {
                             dc.ConsumableName,
                             dc.Specification,
                             Type = d.Type == (int)ConsumableRecordTypeEnums.入库 ? "入库" : "出库",
                             d.Num,
                             d.CreateTime,
                             du.UserName
                         }).ToList();

            //获取当前项目运行的目录
            string path = Directory.GetCurrentDirectory();
            string fileName = "出入库记录导出" + DateTime.Now.ToString("yyyy-MM-dd hh mm ss") + ".xlsx";
            //拼接路径
            string filePath = Path.Combine(path, fileName);

            //Excel
            IWorkbook wk = null;
            string extension = Path.GetExtension(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                if (extension.Equals(".xls"))
                {
                    wk = new HSSFWorkbook();
                }
                else
                {
                    wk = new XSSFWorkbook();
                }

                ISheet sheet = wk.CreateSheet("你想要的名字");

                IRow row = sheet.CreateRow(0);

                #region 创建表头
                //ICell cell = row.CreateCell(0);
                //cell.SetCellValue("耗材名称");

                string[] titleList =
                {
                    "耗材名称",
                    "规格",
                    "出入库类型",
                    "出入库数量",
                    "出入库时间",
                    "操作人",
                };

                //循环添加表头
                for (int i = 0; i < titleList.Length; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(titleList[i]);
                }
                #endregion

                #region 创建表主体
                for (int i = 0; i < datas.Count; i++)
                {
                    var data = datas[i];

                    IRow tempRow = sheet.CreateRow(i + 1);//从第二行开始创建正文

                    ICell tempCell1 = tempRow.CreateCell(0);//每行第一列
                    tempCell1.SetCellValue(data.ConsumableName);

                    ICell tempCell2 = tempRow.CreateCell(1);//每行第二列
                    tempCell2.SetCellValue(data.Specification);

                    ICell tempCell3 = tempRow.CreateCell(2);//每行第三列
                    tempCell3.SetCellValue(data.Type);

                    ICell tempCell4 = tempRow.CreateCell(3);//每行第四列
                    tempCell4.SetCellValue(data.Num);

                    ICell tempCell5 = tempRow.CreateCell(4);//每行第五列
                    tempCell5.SetCellValue(data.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    ICell tempCell6 = tempRow.CreateCell(5);//每行第六列
                    tempCell6.SetCellValue(data.UserName);

                }


                #endregion

                //把Excel写入文件流
                wk.Write(fs);

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                downLoadName = fileName;//返回Excel文件名

                return fileStream;
            }

        }
        #endregion


    }
}
