using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IWorkFlow_InstanceStepBLL
    {
        List<GetWorkFlow_InstanceStepDTO> GetWorkFlow_InstanceStepes(int page, int limit, string userId, out int count);

        bool UpdateWorkFlow_InstanceStep(WorkFlow_InstanceStep workStep,string userId, int outNum,out string msg);

        WorkFlow_InstanceStep GetWorkFlow_InstanceStepById(string id);

        object GetSelectOption();
    }
}
