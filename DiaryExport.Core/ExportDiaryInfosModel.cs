using System.Collections.Generic;

namespace DiaryExport.Core
{
    public class ExportDiaryInfosModel
    {
        public ExportDiaryInfosModel()
        {
            DiaryInfos = new List<DiaryInfo>();
        }
        public List<DiaryInfo> DiaryInfos { get; set; }
    }
}