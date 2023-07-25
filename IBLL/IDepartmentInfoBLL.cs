using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IDepartmentInfoBLL
    {
        List<GetDepartmentInfoDTO> GetDepartmentInfos(int page, int limit, string departmentName, out int count);

        bool DeleteDepartmentInfo(string id);

        bool CreateDepartmentInfo(DepartmentInfo dept, out string msg);

        bool DeleteListDepartmentInfo(List<string> ids);

        bool UpdateDepartmentInfo(DepartmentInfo dept, out string msg);

        object GetSelectOption();

        DepartmentInfo GetDepartmentInfoById(string id);
    }
}
