using System.Collections.Generic;

namespace DiaryExport.Core
{
    public class ExportDiaryInfosWithUserInfoModel
    {
        public ExportDiaryInfosWithUserInfoModel()
        {
            DiaryInfos = new List<DiaryInfo>();
        }
        public UserInfo UserInfo { get; set; }
        public List<DiaryInfo> DiaryInfos { get; set; }
    }
}