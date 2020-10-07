using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DiaryExport.Core;
using DiaryExport.ExportDiaryToFile;

namespace DiaryExport.Cmd
{
    public  class Program
    {
        public static async Task Main(string[] args)
        {
            string email = "";
            string password = "";
            int exportCount = 0;
            bool jsonFileFlag = false;
            bool txtFileFlag = false;
            string txtFilePath = "";
            string jsonFilePath = "";
            try
            {
                Console.Write("输入邮箱：");
                email = Console.ReadLine();
                Console.Write("输入密码：");
                password = Console.ReadLine();
                Console.Write("输入导出日记数量(整数，其中 0 代表全部)：");
                var count = int.TryParse(Console.ReadLine(), out exportCount);

                Console.Write("是否导出 txt 文件(0 否， 1 是):");
                txtFileFlag = Console.ReadLine().Equals("1") ? true: false;
                if (txtFileFlag)
                {
                    Console.Write("设置导出 txt 文件的路径：");
                    txtFilePath = Console.ReadLine();
                }
                Console.Write("是否导出 Json 文件(0 否， 1 是):");
                jsonFileFlag = Console.ReadLine().Equals("1") ? true : false;
                if (jsonFileFlag)
                {
                    Console.Write("输入导出 Json 文件的路径：");
                    jsonFilePath = Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
                Console.ReadKey();
                Environment.Exit(0);
            }
            var diaryInfoList = new ExportDiaryInfosModel();
            var diaryInfoListWithUserInfo = new ExportDiaryInfosWithUserInfoModel();

            if (email.Equals(string.Empty) || password.Equals(string.Empty))
            {
                Console.WriteLine("请输入账号和密码，用于登陆");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var diaryExport = new Core.DiaryExport(email, password,exportCount);

            diaryExport.Login().Wait();

            if (diaryExport.LoginCheck)
            {
                diaryInfoList = await diaryExport.ExportAllDiaryInfo();
                if (diaryInfoList != null && diaryInfoList.DiaryInfos.Count != 0)
                {

                    //foreach (var diaryInfo in diaryInfoList.DiaryInfos)
                    //{
                    //    Console.WriteLine("---------------");
                    //    Console.WriteLine(diaryInfo.Content);
                    //    Console.WriteLine("---------------");
                    //}

                    if (txtFileFlag)
                    {
                        var exportDiaryToTxt = new ExportDiaryToTxtFile(txtFilePath, diaryInfoList);
                        exportDiaryToTxt.StartExport();
                    }

                    if (jsonFileFlag)
                    {
                        var exportDiaryToJson = new ExportDiaryToJsonFile(jsonFilePath, diaryInfoList);
                        exportDiaryToJson.StartExport();
                    }

                }

            }
            else
            {
                Console.WriteLine("登陆失败，请检查账号密码以及查看错误信息");
            }


        }
    }
}
