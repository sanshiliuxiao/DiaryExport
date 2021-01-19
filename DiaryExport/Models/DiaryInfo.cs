using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiaryExport.Models
{
    public class DiaryInfo
    {
        [Key]
        public string Id { get; set; }
        [JsonProperty("deleteddate")]
        public string Deleteddate { get; set; }
        public string Status { get; set; }
        public string Mood { get; set; }
        public string Title { get; set; }
        public string Space { get; set; }
        public DateTime Ts { get; set; }
        public string Content { get; set; }
        [JsonProperty("date_word")]
        public string DataWord { get; set; }
        public string Weather { get; set; }
        public string User { get; set; }
        [JsonProperty("createddate")]
        public DateTime Createddate { get; set; }
        [JsonProperty("createdtime")]
        public DateTime Createdtime { get; set; }

        public string Weekday { get; set; }
    }
}

