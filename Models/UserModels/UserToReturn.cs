namespace WebAPI.Models.UserModels
{
    public class UserToReturn
    {
        public bool authorizated { get; set; }
        public bool customer { get; set; }
        public bool accounter { get; set; }
        public bool realizator { get; set; }
        public bool administrator { get; set; }

        public UserToReturn()
        {
            authorizated = false;
            customer = false;
            accounter = false;
            realizator = false;
            administrator = false;
        }
    }
}