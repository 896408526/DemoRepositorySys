using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class R_UserInfo_RoleInfo : BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [MaxLength(36)]
        public string UserId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        [MaxLength(36)]
        public string RoleId { get; set; }
    }
}
