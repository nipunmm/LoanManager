using LoanManager.Models;

namespace LoanManager.Service
{
    public class AuthService
    {
        private readonly User _hardcodedUser = new User
        {
            Username = "admin",
            Password = "1234"
        };

        public bool ValidateUser(string username, string password)
        {
            return username == _hardcodedUser.Username && password == _hardcodedUser.Password;
        }
    }
}
