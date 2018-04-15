namespace WebAPI.Models.UserModels
{
    public class UserFromDbModel
    {
        public int uid { get; set; }
        public string ulogin { get; set; }
        public string upassword { get; set; }
        public bool uactive { get; set; }
        public bool customer { get; set; }
        public bool accounter { get; set; }
        public bool realizator { get; set; }
        public bool administrator { get; set; }
    }
}
