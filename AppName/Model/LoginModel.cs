using System;
using System.Collections.Generic;
using System.Text;

namespace AppName.Model
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserStoreModel
    {
        public string RetailID { get; set; }
        public string RetailName { get; set; }
        public string Permission { get; set; }
    }

}
