namespace QL_Nha_Khoa
{
    public class CurrentUser
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        private static CurrentUser _instance;
        public static CurrentUser Instance => _instance ??= new CurrentUser();

        private CurrentUser() { }
    }
}
