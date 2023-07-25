using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Model.DTO;
using Model;

namespace IBLL
{
    public interface IUserInfoBLL
    {
        /// <summary>
        /// 登录的方法
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="msg"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool Login(string account, string password, out string msg, out string userName, out string userId);

        /// <summary>
        /// 获取用户列表的方法
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页几行</param>
        /// <param name="userName">姓名</param>
        /// <param name="account">账号</param>
        /// <param name="count">共多少条数据</param>
        /// <returns></returns>
        List<GetUserInfoDTO> GetUserInfoes(int page,int limit,string userName, string account,out int count);

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        bool CreateUserInfo(UserInfo user, out string msg);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        bool DeleteUserInfo(string id);

        /// <summary>
        /// 修改的方法
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool UpdateUserInfo(UserInfo user, out string msg);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        bool DeleteListUserInfo(List<string> ids);

        /// <summary>
        /// 获取部门下拉框的方法
        /// </summary>
        /// <returns></returns>
        object GetSelectOption();

        /// <summary>
        /// 根据ID获取数据返回赋值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserInfo GetUserInfoById(string id);

        /// <summary>
        /// 修改账号密码的方法
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool UpdatePassWord(string id, string oldPwd,string newPwd, string agPwd, out string msg);

        /// <summary>
        /// 设置个人信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SettingUser(UserInfo user, out string msg);

        /// <summary>
        /// 获取全部用户的方法
        /// </summary>
        /// <returns></returns>
        List<GetUserInfoDTO> GetUserInfoDTOs();

    }
}
