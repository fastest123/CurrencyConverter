using CurrencyConverter.Models;
using static CurrencyConverter.Common.Resources;

namespace CurrencyConverter.IServices.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse<Token>> Authenticate(Login loginModel);
    }
}
