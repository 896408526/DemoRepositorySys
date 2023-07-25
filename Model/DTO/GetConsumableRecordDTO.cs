using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class GetConsumableRecordDTO
    {
        public string Id { get; set; }
        public string ConsumableId { get; set; }
        public int Num { get; set; }
        public int Type { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
