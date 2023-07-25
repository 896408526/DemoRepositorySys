using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class WorkFlow_Model : BaseDeleteEntity
    {
        /// <summary>
        /// 模板标题
        /// </summary>
        [MaxLength(36)]
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(64)]
        public string Description { get; set; }
    }
}
