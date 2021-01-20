using System;
using System.Collections.Generic;
using System.Text;

namespace DiaryExport.ModelDtos
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Csrf { get; set; } = "ZcJkWtMdfyjBNKxp3ms0i8REkTJcSKw4";
        public string Token { get; set; } = "token ";
        public int ExportCount { get; set; } = int.MaxValue;
        public LoginModel():this("", "")
        {

        }
        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
