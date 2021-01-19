using DiaryExport.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiaryExport.ModelDtos
{
    public class DiaryModel
    {
        public string Version { get; set; }
        public DiaryInfo Diary { get; set; }
        public int Error { get; set; }
    }
}
