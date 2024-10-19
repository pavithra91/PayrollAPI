using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using PayrollAPI.Authentication;
using PayrollAPI.Data;
using PayrollAPI.Interfaces;
using PayrollAPI.Repository;
using PayrollAPI.Services;
using PdfSharp.Charting;
using System.Reflection;
using System.Text;


// Initialize Logs
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger(); // NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    // Disable Cors
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    var builder = WebApplication.CreateBuilder(args);

    var env = builder.Environment.EnvironmentName;

    // Disable Cors
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.AllowAnyOrigin().AllowAnyHeader()
                                                      .AllowAnyMethod();
                          });
    });


    // Add services to the container.

    if(env == "Production")
    {
        IAmazonSecretsManager amazonSecretsManager = new AmazonSecretsManagerClient("AKIAV3CJE2DCBB7UZJDM", "oPvNVvN3U5e+MZwtmRK8/X+5kLDxNzXsCubr1XbT", RegionEndpoint.APSoutheast1);

        var request = new GetSecretValueRequest
        {
            SecretId = $"{env}_PayrollAPI_ConnectionStrings__LocalConnection"
        };

        var result = await amazonSecretsManager.GetSecretValueAsync(request);

        var connectionStringParts = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.SecretString);
        var username = connectionStringParts["username"];
        var password = connectionStringParts["password"];
        var engine = connectionStringParts["engine"];
        var host = connectionStringParts["host"];
        var port = int.Parse(connectionStringParts["port"]);
        var dbClusterIdentifier = connectionStringParts["dbClusterIdentifier"];

        var connectionStringTemplate = $"Server={host};Port={port};Database={dbClusterIdentifier};Uid={username};Pwd={password};";


        builder.Services.AddDbContext<PayrollAPI.Data.DBConnect>(options =>
        options.UseMySQL(connectionStringTemplate));
    }
    else
    {
        builder.Services.AddDbContext<PayrollAPI.Data.DBConnect>(options =>
        options.UseMySQL(builder.Configuration.GetConnectionString("DevLocalConnection")));
    }

    var _dbContext = builder.Services.BuildServiceProvider().GetService<DBConnect>();

    // Add health check
    builder.Services.AddHealthChecks().AddDbContextCheck<PayrollAPI.Data.DBConnect>();

    builder.Services.AddSingleton<IRefreshTokenGenerator>(provider => new RefreshTokenGenerator(_dbContext));


    builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
    builder.Services.AddHostedService<BackgroudService>();


    var _jwtsetting = builder.Configuration.GetSection("JWTSetting");
    builder.Services.Configure<JWTSetting>((IConfiguration)_jwtsetting);

    var authkey = builder.Configuration.GetValue<string>("JWTSetting:securitykey");

    builder.Services.AddAuthentication(item =>
    {
        item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(item =>
    {

        item.RequireHttpsMetadata = true;
        item.SaveToken = true;
        item.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


    // Add NLogger to Builder
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();



    builder.Services.AddAuthorization();

    builder.Services.AddControllers();

    builder.Services.AddScoped<IUsers, UsersRepository>();
    builder.Services.AddScoped<IDatatransfer, DataRepository>();
    builder.Services.AddScoped<IPayroll, PayrollReporsitory>();
    builder.Services.AddScoped<IAdmin, AdminRepository>();
    builder.Services.AddScoped<IHelp, HelpRepository>();

    builder.Services.AddScoped<ApiKeyAuthFilter>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CPSTL Payroll API",
            Version = "v1",
            Description = "CPSTL Payroll API is a robust tool designed to efficiently handle and process company's payroll data.",
            Contact = new OpenApiContact
            {
                Name = "R.A.P.B.M Jayasundara",
                Email = "pavi.dsscst@gmail.com",
                Url = new Uri("https://www.linkedin.com/in/pavithra-jayasundara/"),
            }
        });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CPSTL Payroll API V1");
            });
    }

    app.UseHttpsRedirection();

    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    });

    // Disable Cors
    app.UseCors(MyAllowSpecificOrigins);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex);
    throw (ex);
}
finally
{
    NLog.LogManager.Shutdown();
}
