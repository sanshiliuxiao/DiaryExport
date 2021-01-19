using DiaryExport.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiaryExport.ModelDtos
{
    public class UserModel
    {
        public string Token { get; set; } = "token ";
        [JsonProperty("user_config")]
        public UserInfo UserConfig { get; set; }
        public int Error { get; set; }
    }
}
