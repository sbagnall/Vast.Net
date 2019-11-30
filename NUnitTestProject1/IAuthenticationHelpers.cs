using System.Net.Http;
using Moq;

namespace NUnitTestProject1
{
    public interface IAuthenticationHelpers
    {
        Mock<HttpMessageHandler> StubAuthentication(bool success = true, string username = "test@account.com", string password = "Password");
    }
}