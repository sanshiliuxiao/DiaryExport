using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace DiaryExport.Models
{
    public class UserInfo
    {
        [Key]
        [JsonProperty("userid")]
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("useremail")]
        public string UserEmail { get; set; }
        [JsonProperty("diary_count")]
        public int DiaryCount { get; set; }
        [JsonProperty("word_count")]
        public int WordCount { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
    }
}
