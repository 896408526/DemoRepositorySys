using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{

    public interface IWorkFlow_ModelBLL
    {
        List<GetWorkFlow_ModelDTO> GetWorkFlow_Modeles(int limit, int page, string title, out int count);

        bool CreateWorkFlow_Model(WorkFlow_Model workmode, out string msg);

        bool DeleteWorkFlow_Model(string id);

        bool DeleteListWorkFlow_Model(List<string> ids);

        bool UpdateWorkFlow_Model(WorkFlow_Model workmode, out string msg);
    }
}
