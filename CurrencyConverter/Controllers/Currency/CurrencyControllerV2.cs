using CurrencyConverter.IServices.Currency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Controllers.Currency
{
    [ApiVersion("2")]
    [ApiController]
    [Authorize]
    [Route("v{version:apiVersion}/currency")]
    public class CurrencyControllerV2 : Controller
    {
        private readonly ICurrencyProvider _currencyProvider;
        public CurrencyControllerV2(ICurrencyProvider currencyProvider)
        {
            _currencyProvider = currencyProvider;
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
        /// Get /v2/currency/convert_amount?10.0?USD
        /// 
        /// </remarks>
        [HttpGet("convert_amount")]
      //  [MapToApiVersion("2")]

        public async Task<IActionResult> ConvertAmount(decimal amount, string FCurrency, string BaseCurrency = "EUR", string provider = "frankfurter")
        {
            string data = null;
            
                var currProvider = _currencyProvider.GetCurrencyProvider(provider);
                var BaseAmount = await currProvider.ConvertAmount(amount, FCurrency.ToUpper(), BaseCurrency.ToUpper());
                return Ok(BaseAmount);
           
        }

    }
}
