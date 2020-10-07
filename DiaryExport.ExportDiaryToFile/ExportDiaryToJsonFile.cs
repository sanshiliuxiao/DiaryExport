using System.IO;
using System.Text;
using DiaryExport.Core;
using Newtonsoft.Json;

namespace DiaryExport.ExportDiaryToFile
{
    public class ExportDiaryToJsonFile
    {
        private string _filePath;
        private readonly ExportDiaryInfosModel _diaryInfos;

        public ExportDiaryToJsonFile(string filePath, ExportDiaryInfosModel diaryInfos)
        {
            _filePath = filePath;
            _diaryInfos = diaryInfos;
        }

        public void StartExport()
        {
            var fileInfo = new FileInfo(_filePath);

            if (fileInfo.Extension == string.Empty)
            {
                _filePath = $@"{_filePath}\ExportDiary.json";
            }

            string json = JsonConvert.SerializeObject(_diaryInfos, Formatting.Indented);
            File.WriteAllText(_filePath, json, Encoding.UTF8);
        }
    }
}