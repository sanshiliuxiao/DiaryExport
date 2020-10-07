namespace DiaryExport.Core
{
    public class UserModel
    {
        public string Token { get; set; }
        public UserInfo User_config { get; set; }

        public int error { get; set; }
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public string Useremail { get; set; }
        public int Diary_count { get; set; }
        public int Word_count { get; set; }
        public string Desription { get; set; }
        public string Avatar { get; set; }
    }
}