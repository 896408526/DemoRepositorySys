using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class GetConsumableInfoDTO
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string ConsumableName { get; set; }
        public string Specification { get; set; }
        public int Num { get; set; }
        public string Unit { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
