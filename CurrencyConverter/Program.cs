using CurrencyConverter.Factories;
using CurrencyConverter.IServices.Auth;
using CurrencyConverter.IServices.Currency;
using CurrencyConverter.Models;
using CurrencyConverter.Services.Auth;
using CurrencyConverter.Services.Currency;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CurrencyConverter.Middleware;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

// Set up Serilog to read from appsettings.json
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration) // Read from appsettings.json
//    .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // Log events of this level or higher
    .WriteTo.Console()  // Log to console (for local debugging)
    .WriteTo.Seq("http://localhost:5341")  // Send logs to Seq (replace with your Seq server URL)
    .Enrich.FromLogContext()  // Add contextual information to logs
    .Enrich.WithProperty("Application", "CurrencyApi")  // Add a custom property
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging
// Add services to the container.

// Api versioning
builder.Services.AddControllers();
builder.Services.AddApiVersioning(x => {
    x.DefaultApiVersion = new ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                          new HeaderApiVersionReader("x-api-version"),
                                                                          new MediaTypeApiVersionReader("x-api-version"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Authorize checking in swagger
builder.Services.AddSwaggerGen(c =>
{


    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "V1",
        Title = "BambooTest API",
        Description = "BambooTest API"
    });
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "V2",
        Title = "BambooTest API V2",
        Description = "BambooTest API V2"
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
    {
        Description =
                          "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    var oar = new OpenApiSecurityRequirement();
   oar.Add(
       new OpenApiSecurityScheme
       {
           Reference = new OpenApiReference
           {
               Type = ReferenceType.SecurityScheme,
               Id = "bearerAuth"
           },
           Scheme = "oauth2",
           Name = "Bearer",
           In = ParameterLocation.Header,
       }, new List<string>());
   c.AddSecurityRequirement(oar);
   var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
   var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
   c.IncludeXmlComments(xmlPath);
    //xml comments
}
);



var jwtSettings = builder.Configuration.GetSection("JWTSettings:SecretKey");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.Value);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        };
    });

        


builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddHttpClient<Frankfurter>();
builder.Services.AddSingleton<ICurrencyProvider, CurrencyProvider>();

//Role based authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();



//app.UseHttpsRedirection();
app.UseAuthentication();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BambooTest API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "BambooTest API v2");
    });
}
//app.UseSerilogRequestLogging();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.MapControllers();
app.UseAuthorization();
app.Run();
