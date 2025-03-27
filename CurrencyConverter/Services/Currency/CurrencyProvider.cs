using CurrencyConverter.Factories;
using CurrencyConverter.IServices.Currency;

namespace CurrencyConverter.Services.Currency
{
    public class CurrencyProvider : ICurrencyProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public CurrencyProvider( IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ICurrencyService GetCurrencyProvider(string providerType)
        {
           if(providerType.ToLower() == "frankfurter")
            {
                return _serviceProvider.GetRequiredService<Frankfurter>();

            }
            throw new ArgumentException($"Unknown provider type: {providerType}");
        }
    }
}
