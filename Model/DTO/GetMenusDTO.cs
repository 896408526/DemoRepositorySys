using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    /// <summary>
    /// 获取首页菜单的对象
    /// </summary>
    public class GetMenusDTO
    {
        public GetMenusDTO()
        {

        }

        public GetMenusDTO(List<HomeMenuInfoDTO> menus)
        {
            var homeMenuInfoDTO = this.MenuInfo.FirstOrDefault();
            if(homeMenuInfoDTO != null)
            {
                homeMenuInfoDTO.Child = menus;
            }
        }

        public HomeMenuInfoDTO HomeInfo { get; set; } = new HomeMenuInfoDTO()
        {
            Title = "首页",
            Href = "../layuimini/page/welcome-1.html?t=1"
        };

        public HomeMenuInfoDTO LogoInfo { get; set; } = new HomeMenuInfoDTO()
        {
            Title = "坤坤管理系统",
            Image = "../layuimini/images/logo.png",
            Href = ""
        };

        public List<HomeMenuInfoDTO> MenuInfo { get; set; } = new List<HomeMenuInfoDTO>()
        {
            new HomeMenuInfoDTO()
            {
                Title =  "常规管理",
                Icon = "fa fa-address-book",
                Href = "",
                Target = "_self",
            }
        };
    }
}
