using ChatApp.API.Untity;

namespace ChatApp.API.Services
{
    public interface ILoginService
    {
        public bool ValidateLoginInfo(User user);
    }
}
