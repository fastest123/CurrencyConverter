Currency Converter


1.   To generate JWT Token for API Auth. /v1/auth/token For testing, you can use { "grantType": "password", "username": "Test", "password": "Test123" } { "grantType": "password", "username": "Card", "password": 
     "CardTest" } To get an access token using the password grant type, send a POST request to the /auth/token endpoint, passing the application credentials in the request body. Hardcoded 2 users for testing 
     purpose. In future will validate the credentials with database and retrieve the logged in user details.

2.   To fetch the full list of latest exchange rates for a specific base currency (default :EUR) /v1/currency/exchange_rates?BaseCurrency=USD&provider=frankfurter
3.   To convert amounts between different currencies /v1/currency/convert_amount?amount=120&FCurrency=usd&BaseCurrency=inr&provider=frankfurter
4.   Retrieve historical exchange rates for a given period with pagination (e.g., 2020-01-01 to 2020-01-31, base EUR). /v1/currency/history?start=2025-03-20&end=2025-03-27&page=1&pageSize=10&provider=frankfurter
5.   For checking Role based access control (RBAC) /v1/currency/admin_convert_amount?amount=100&FCurrency=usd&BaseCurrency=EUR&provider=frankfurter
6.   For Version checking /v1/currency/convert_amount?amount=120&FCurrency=usd&BaseCurrency=inr&provider=frankfurter /v2/currency/convert_amount?amount=120&FCurrency=usd&BaseCurrency=inr&provider=frankfurter

     Caching : In memory caching implemented using memoryCache to minimize the exchange rate for base and To-currency to calculate the amount.
     By Default - currency provider is - frankfurter, Factory pattern implementation will allow to integrate multiple exchange rate providers.
     Serilog logging implemented to log the response to a file located in "logs/api_log.txt".
