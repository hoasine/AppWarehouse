using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppName.ViewModels.BarCode.Model
{
    public partial class LoginModel
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string IsMarkdown { get; set; }
        public string RoleID { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Mess { get; set; }
        public string DateExpires { get; set; }
        public List<PermissionModel> PermissionList { get; set; }
    }

    public partial class PermissionModel
    {
        public string KeyPermission { get; set; }
        public string Role { get; set; }
    }
}
