using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Category : BaseDeleteEntity
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        [MaxLength(16)]
        public string CategoryName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(32)]
        public string Description { get; set; }
    }
}
