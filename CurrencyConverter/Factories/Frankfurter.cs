using CurrencyConverter.IServices.Currency;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using static CurrencyConverter.Common.Resources;
using CurrencyConverter.Models;

namespace CurrencyConverter.Factories
{
    public class Frankfurter : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        

        public Frankfurter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<Dictionary<string,decimal>>> GetExchangeRate(string BaseCurrency)
        {
            var apiResponse =  new ServiceResponse<Dictionary<string,decimal>>();
            var rateUrl = "https://api.frankfurter.dev/v1/latest?base="+BaseCurrency;
            try
            {
                // Make the API request
                var response = await _httpClient.GetStringAsync(rateUrl);

               // Deserialize the API response
                var exchangeRates = JsonConvert.DeserializeObject<FrankfurterModel>(response);

                if(exchangeRates != null && exchangeRates.Rates != null)
                {
                    apiResponse.Status = StaticResource.SuccessStatusCode;
                    apiResponse.Message = StaticResource.SuccessMessage;
                    apiResponse.Result = exchangeRates.Rates;

                }
                else
                {
                    apiResponse.Status = StaticResource.NotFoundStatusCode;
                    apiResponse.Message = StaticResource.NotFoundMessage;
                    apiResponse = new ServiceResponse<Dictionary<string,decimal>>();
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = StaticResource.FailStatusCode;
                apiResponse.Message = ex.Message;
                apiResponse = new ServiceResponse<Dictionary<string, decimal>>();
            }
            return  apiResponse;
        }

        public async Task<ServiceResponse<decimal>> GetExchangeRateByCurrency(string BaseCurrency, string ToCurrency)
        {
            var rateUrl = "https://api.frankfurter.dev/v1/latest";
            string cacheKey = BaseCurrency+"_"+ToCurrency;
            ServiceResponse<decimal> apiResponse = new ServiceResponse<decimal>();
            try
            {
                // Check if the value is cached
                if (_cache.TryGetValue(cacheKey, out decimal cachedResult))
                {
                    apiResponse.Status = StaticResource.SuccessStatusCode;
                    apiResponse.Message = StaticResource.SuccessMessage;
                    apiResponse.Result = Math.Round(cachedResult, 2);
                }
                // Make the API request
                var response = await _httpClient.GetStringAsync(rateUrl);

                // Deserialize the API response
                var exchangeRates = JsonConvert.DeserializeObject<FrankfurterModel>(response);

                if (exchangeRates.Rates != null)
                {
                    if (exchangeRates.Rates.ContainsKey(ToCurrency))
                    {
                        _cache.Set(cacheKey,Math.Round( exchangeRates.Rates[ToCurrency],2), TimeSpan.FromMinutes(10));
                        apiResponse.Status = StaticResource.SuccessStatusCode;
                        apiResponse.Message = StaticResource.SuccessMessage;
                        apiResponse.Result =Math.Round( exchangeRates.Rates[ToCurrency],2);
                    }
                    else
                    {
                        apiResponse.Status = StaticResource.NotFoundStatusCode;
                        apiResponse.Message = StaticResource.NotFoundMessage;
                        apiResponse.Result = 0;

                    }
                }
                else
                {
                    apiResponse.Status = StaticResource.NotFoundStatusCode;
                    apiResponse.Message = StaticResource.NotFoundMessage;
                    apiResponse.Result = 0;

                }
                
                  
            }
            catch (Exception ex)
            {
                    apiResponse.Status = StaticResource.FailStatusCode;
                    apiResponse.Message = ex.Message;
                    apiResponse.Result = 0;
            }
            return apiResponse;
        }

        public async Task<ServiceResponse< decimal>> ConvertAmount(decimal amount, string FCurrency, string BaseCurrency)
        {
            ServiceResponse<decimal> rateResponse = new ServiceResponse<decimal>();
            ServiceResponse<decimal> response = new ServiceResponse<decimal>();
            try
            {
                if (!VerifyExcludedCurrencies(BaseCurrency, FCurrency))
                {
                    rateResponse = await GetExchangeRateByCurrency(BaseCurrency, FCurrency);
                    response.Status = StaticResource.SuccessStatusCode;
                    response.Message = StaticResource.SuccessMessage;
                    response.Result = amount * rateResponse.Result;
                }
                else
                {
                    response.Status = StaticResource.FailStatusCode;
                    response.Message = StaticResource.ExcludedCurrencies;
                    response.Result = 0;
                }
            }
            catch(Exception ex)
            {
                response.Status = StaticResource.FailStatusCode;
                response.Message = ex.Message;
                response.Result = 0;
            }
            return response;
        }
       
        public async Task<ServiceResponse<Dictionary<DateTime, Dictionary<string, decimal>>>> GetHistory(DateTime start, DateTime end, int page, int pageSize)
        {
            ServiceResponse<Dictionary<DateTime, Dictionary<string, decimal>>> apiresponse = new ServiceResponse<Dictionary<DateTime, Dictionary<string, decimal>>>();
            var historyUrl = "https://api.frankfurter.dev/v1/{start}..{end}";
            var results = new Dictionary<DateTime, Dictionary<string, decimal>>();

            try
            {
                var totalDays = (end - start).Days;
                var startPage = (page - 1) * pageSize;
                var endPage = Math.Min(startPage + pageSize - 1, totalDays);

                for (var i = startPage; i <= endPage; i++)
                {
                    var currentDate = start.AddDays(i);

                    var dailyUrl = "https://api.frankfurter.dev/v1/" + currentDate.ToString("yyyy-MM-dd");
                    var response = await _httpClient.GetStringAsync(dailyUrl);
                    var exchangeRates = JsonConvert.DeserializeObject<FrankfurterModel>(response);
                    results[currentDate] = exchangeRates?.Rates ?? new Dictionary<string, decimal>();
                }

                apiresponse.Status = StaticResource.SuccessStatusCode;
                apiresponse.Message = StaticResource.SuccessMessage;
                apiresponse.Result = results;
            }
            catch (Exception ex)
            {
                apiresponse.Status = StaticResource.FailStatusCode;
                apiresponse.Message = ex.Message;
            }
            return apiresponse;

        }
     
        private bool VerifyExcludedCurrencies(string BaseCurrency, string FCurrency)
        {
            var excludedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };
            if (Array.Exists(excludedCurrencies, c => c.Equals(BaseCurrency) || Array.Exists(excludedCurrencies, c => c.Equals(FCurrency))))
            {
                return true;
            }
            else
                return false;
        }
    }
}
