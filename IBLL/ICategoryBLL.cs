using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.DTO;

namespace IBLL
{
	public interface ICategoryBLL
	{
		List<GetCategoryDTO> GetCategoryes(int limit, int page, string categoryName, out int count);

		bool CreateCategory(Category cate, out string msg);

		bool DeleteCategory(string id);

		bool DeleteListCategory(List<string> ids);

		bool UpdateCategory(Category cate, out string msg);
	}
}
