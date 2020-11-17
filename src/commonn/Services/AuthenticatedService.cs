namespace Common.Services
{
    public interface IAuthenticatedService
    {
        string GetUserKey();
        string GetUserName();
    }

    public class AuthenticatedService : IAuthenticatedService
    {
        public string GetUserKey()
        {
            return "Id";
        }

        public string GetUserName()
        {
            return "Name";
        }
    }
}
