namespace CheckersService
{
    public class UserContact
    {
        public UserContact(string userName, ICheckersCallback checkersCallback)
        {
            UserName = userName;
            CheckersCallback = checkersCallback;
        }
        public string UserName { get; }
        public ICheckersCallback CheckersCallback { get; }
    }
}