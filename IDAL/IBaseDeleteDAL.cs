using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IBaseDeleteDAL<T> : IBaseDAL<T> where T : BaseDeleteEntity
    {
        /// <summary>
        /// 获取单个对象根据ID获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetEntityById(string id);
    }
}
