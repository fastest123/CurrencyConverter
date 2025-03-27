namespace CurrencyConverter.Common
{
    public class Resources
    {
        public class ServiceResponse<T>
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public T Result { get; set; }
            public string Information { get; set; }
        }

        public static class StaticResource
        {
            public const int SuccessStatusCode = 200;
            public const string SuccessMessage = "Success";

            public const int NotFoundStatusCode = 404;
            public const string NotFoundMessage = "No record found";

            public const int UnauthorizedStatusCode = 401;
            public const string UnauthorizedMessage = "Unauthorized";
            
            public const int FailStatusCode = 500;
            public const string FailMessage = "Error occurred";

            public const string ExcludedCurrencies = "Excluded Currencies";
        }
    }
}
