using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IWorkFlow_InstanceBLL
    {
        List<GetWorkFlow_InstanceDTO> GetWorkFlow_Instances(int page, int limit,string userId, out int count);

        bool CreateWorkFlow_Instance(WorkFlow_Instance param, string userId, out string msg);

        bool UpdateWorkFlow_Instance(WorkFlow_Instance workisce, out string msg);

        WorkFlow_Instance GetWorkFlow_InstanceById(string id);

        object GetSelectOption();
    }
}
