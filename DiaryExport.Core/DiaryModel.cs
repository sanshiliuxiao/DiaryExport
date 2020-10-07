using System;

namespace DiaryExport.Core
{
    public class DiaryModel
    {
        public string Version { get; set; }
        public DiaryInfo Diary { get; set; }
        public int Error { get; set; }
    }

    public class DiaryInfo
    {
        public string Deleteddate { get; set; }
        public string Status { get; set; }
        public string Mood { get; set; }
        public string Title { get; set; }
        public string Space { get; set; }
        public DateTime Ts { get; set; }
        public string Content { get; set; }
        public string Date_word { get; set; }
        public string weather { get; set; }
        public DateTime Createddate { get; set; }
        public string Id { get; set; }
        public string Weekday { get; set; }
    }
}