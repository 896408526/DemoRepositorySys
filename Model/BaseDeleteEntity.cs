using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 带有软删除的基类
    /// </summary>
    public class BaseDeleteEntity : BaseEntity
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 删除时间(？可以为空)
        /// </summary>
        public DateTime? DeleteTime { get; set; }
    }
}
