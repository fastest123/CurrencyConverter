using static CurrencyConverter.Common.Resources;

namespace CurrencyConverter.IServices.Currency
{
    public interface ICurrencyService
    {
        Task<ServiceResponse<Dictionary<string,decimal>>> GetExchangeRate(string BaseCurrency);
        Task<ServiceResponse<decimal>> GetExchangeRateByCurrency(string ToCurrency,string BaseCurrency);

        Task<ServiceResponse<decimal>> ConvertAmount(decimal Amount, string ToCurrency,string BaseCurrency);

        Task<ServiceResponse<Dictionary<DateTime, Dictionary<string, decimal>>>> GetHistory(DateTime start, DateTime end, int page, int pageSize);
    }
}
