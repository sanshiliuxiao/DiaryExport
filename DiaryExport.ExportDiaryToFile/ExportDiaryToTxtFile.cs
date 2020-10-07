using System;
using System.IO;
using System.Text;
using DiaryExport.Core;

namespace DiaryExport.ExportDiaryToFile
{
    public class ExportDiaryToTxtFile
    {
        private  string _filePath;
        private readonly ExportDiaryInfosModel _diaryInfos;

        public ExportDiaryToTxtFile(string filePath, ExportDiaryInfosModel diaryInfos)
        {
            _filePath = filePath;
            _diaryInfos = diaryInfos;
        }
        public void StartExport()
        {
            var fileInfo = new FileInfo(_filePath);

            if (fileInfo.Extension == string.Empty)
            {
                _filePath = $@"{_filePath}\ExportDiary.txt";
            }
            var txt = "";
            foreach (var diaryInfo in _diaryInfos.DiaryInfos)
            {
                if (diaryInfo.Deleteddate.Equals("None") || !diaryInfo.Content.Equals("deleted"))
                {
                    txt += Environment.NewLine;
                    txt += $"标题: {diaryInfo.Title}" + Environment.NewLine;
                    txt += $"创建日期: {diaryInfo.Createddate}" + Environment.NewLine;
                    txt += $"更新日期: {diaryInfo.Ts}" + Environment.NewLine;
                    txt += $"内容： " + Environment.NewLine + Environment.NewLine;
                    txt += diaryInfo.Content + Environment.NewLine + Environment.NewLine;
                    txt += "----------我是分隔符----------" + Environment.NewLine;
                    txt += Environment.NewLine;
                }
                
            }
            File.WriteAllText(_filePath, txt, Encoding.UTF8);
        }
    }
}