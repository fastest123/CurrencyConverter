using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CurrencyConverter.IServices.Currency;
using Microsoft.AspNetCore.Authorization;

namespace CurrencyConverter.Controllers.Currency
{
    [ApiVersion("1")]
    [ApiController]
    [Authorize]
    [Route("v{version:apiVersion}/currency")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyProvider _currencyProvider;
        public CurrencyController(ICurrencyProvider currencyProvider)
        {
            _currencyProvider = currencyProvider;
        }

        /// <summary>
        /// Get the list of the exchange rate from Frankfurter API for a specific base currency
        /// </summary>
        /// <remarks>
        ///  Retrieve the list of exchange rates
        ///
        ///   Sample Request:
        ///
        ///     GET /v1/currency/exchange_rates?USD
        ///
        /// </remarks>
        [HttpGet("exchange_rates")]
        public async Task<IActionResult> GetRate(string BaseCurrency="EUR", string provider="frankfurter")
        {
            var currProvider = _currencyProvider.GetCurrencyProvider(provider);

            var exchangeRate = await currProvider.GetExchangeRate(BaseCurrency);
            return Ok(exchangeRate);
        }

        /// <summary>
        /// Get the exchange rate by passing Base currency and Foreign currency 
        /// </summary>
        /// <remarks>
        /// Retrieve the currency rate by passing from and to currencies
        /// 
        /// Sample Request :
        /// 
        ///     Get /v1/currency/exchange_rates_curr?USD
        /// </remarks>
        [HttpGet("exchange_rates_curr")]
        public async Task<IActionResult> GetRateByCurrency( string ToCurrency,string BaseCurrency="EUR", string provider = "frankfurter")
        {
            var currProvider = _currencyProvider.GetCurrencyProvider(provider);

            var exchangeRate = await currProvider.GetExchangeRateByCurrency(BaseCurrency.ToUpper(),ToCurrency.ToUpper());
            return Ok(exchangeRate);
        }


        /// <summary>
        /// 
        /// Convert Amounts between different currencies
        /// 
        /// </summary>
        ///<remarks>
        ///
        /// To Convert given amount to different currencies
        /// 
        /// Sample Request
        /// 
        /// Get /v1/currency/convert_amount?10.0?USD
        /// 
        /// </remarks>
        [HttpGet("convert_amount")]
                
        public async Task<IActionResult> ConvertAmount(decimal amount, string FCurrency, string BaseCurrency="EUR", string provider = "frankfurter")
        {
            string data = null;
            
                var currProvider = _currencyProvider.GetCurrencyProvider(provider);
                var BaseAmount = await currProvider.ConvertAmount(amount, FCurrency.ToUpper(), BaseCurrency.ToUpper());
               return Ok(BaseAmount);

        }

        /// <summary>
        /// To get the history of exchange rates
        /// </summary>
       ///<remarks>
       ///
       ///  Retrieve historical exchange rates for a given period with pagination (e.g., 2020-01-01 to 2020-01-31, base EUR).
       ///  
       /// Sample Request :
       /// Get v1/currency/history?2025-03-20?2025-03-27?1?10
       /// </remarks>
        [HttpGet("history")]
        public async Task<IActionResult> History(DateTime start, DateTime end, int page=1, int pageSize=10, string provider="frankfurter")
        {
            var currProvider = _currencyProvider.GetCurrencyProvider(provider);
            var ratehistory = await currProvider.GetHistory(start, end, page,pageSize);
            return Ok(ratehistory);

        }


        /// <summary>
        /// 
        /// Convert Amounts between different currencies
        /// 
        /// </summary>
        ///<remarks>
        ///
        /// To Check the RBAC only for admins - Convert given amount to different currencies
        /// 
        /// Sample Request
        /// 
        /// Get /v1/currency/admin_convert_amount?10.0?USD
        /// 
        /// </remarks>
        [HttpGet("admin_convert_amount")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(string))] // OK response
        [ProducesResponseType(403, Type = typeof(void))]  // Forbidden response

        public async Task<IActionResult> ConvertAmountAdmin(decimal amount, string FCurrency, string BaseCurrency = "EUR", string provider = "frankfurter")
        {       
                var currProvider = _currencyProvider.GetCurrencyProvider(provider);
                var BaseAmount = await currProvider.ConvertAmount(amount, FCurrency.ToUpper(), BaseCurrency.ToUpper());
                return Ok(BaseAmount);
           
        }

    }
}
