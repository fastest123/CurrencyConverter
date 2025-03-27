namespace CurrencyConverter.IServices.Currency
{
    public interface ICurrencyProvider
    {
        ICurrencyService GetCurrencyProvider(string provider);
    }
}
