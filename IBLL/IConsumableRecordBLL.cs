using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IConsumableRecordBLL
    {
        List<GetConsumableRecordDTO> GetConsumableRecordes(int page, int limit, out int count);
        
        bool DeleteListConsumableRecord(List<string> ids);
    }
}
