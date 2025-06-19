using System.Globalization;
using System.Text.Json.Serialization;

using GenApi;
using GenApi.Repositories;
using GenApi.Services;
using GenApi.Services.RabbitMQ;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using OpenSearch.Client;

using Scalar.AspNetCore;

using Serilog.Sinks.OpenSearch;

var builder = WebApplication.CreateBuilder(args);

// * Config ISO8601 CultureInfo, với format dd-MM-yyyy HH:mm:ss mặc định
CultureInfo cultureInfo = new("vi-VN"); // chỉnh lại culture thành vietnam
cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd"; // định dạng ngày default
cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss"; // định dạng thời gian default
CultureInfo.DefaultThreadCurrentCulture = cultureInfo; // chỉnh lại trên source
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo; // chỉnh lại trên source

// * map appsettings.json tại property AppSettings với class AppSettings
IConfigurationSection appSettingsConfig = builder.Configuration.GetSection(nameof(AppSettings));
builder.Services.Configure<AppSettings>(appSettingsConfig);
AppSettings? appSettings = appSettingsConfig.Get<AppSettings>();

// * Serilog
// đọc serilog config từ file appsettings.json hoặc file appsettings.*.json, tùy vào môi trường api đang chạy
string environment = builder.Environment.EnvironmentName;
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// config lại logger của api
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithComputed("SourceContext", "Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)") // chỗ placeholder SourceContext sẽ log tên class mà ko log namespace của log
                                                                                                           // config log vào opensearch
    .WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(appSettings?.OpenSearch?.NodeUris ?? string.Empty))
    {
        ModifyConnectionSettings = x => x.BasicAuthentication(appSettings?.OpenSearch?.UserName ?? string.Empty, appSettings?.OpenSearch?.Password ?? string.Empty)
                .ServerCertificateValidationCallback((sender, cert, chain, sslPolicyErrors) => true),
        AutoRegisterTemplate = true, // Automatically register index template
        IndexFormat = $"gen-api-logs-{DateTime.Now:yyyy.MM.dd}", // Daily index
        TypeName = null, // For OpenSearch 2.x and above, this should be null
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog | EmitEventFailureHandling.RaiseCallback,
        FailureCallback = e => Console.WriteLine($"Failed to log to OpenSearch: {e.ToJsonString()}")
    })
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecurityTransformer>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(appSettings?.ConnectionStrings));


builder.Services.AddMassTransit(x =>
{
    // Đăng ký 1 consumer giống Services.AddScoped<SubmitAnimeConsumer>();
    x.AddConsumer<SubmitAnimeConsumer>();

    // Configure RabbitMQ as the transport
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(appSettings?.RabbitMQ?.Host, (ushort)(appSettings?.RabbitMQ?.Port ?? 0), "/", h =>
        {
            h.Username(appSettings?.RabbitMQ?.UserName ?? string.Empty);
            h.Password(appSettings?.RabbitMQ?.Password ?? string.Empty);
        });

        // Configure the receive endpoint (queue)
        // Config nhận message từ queue anime-queue và inject Service SubmitAnimeConsumer để nhận
        cfg.ReceiveEndpoint(appSettings?.RabbitMQ?.QueueName ?? string.Empty, e =>
        {
            e.ConfigureConsumer<SubmitAnimeConsumer>(context);
        });
    });
});

// * open search
var settings = new ConnectionSettings(new Uri(appSettings?.OpenSearch?.NodeUris ?? string.Empty)) // use container name as hostname!
    .BasicAuthentication(appSettings?.OpenSearch?.UserName ?? string.Empty, appSettings?.OpenSearch?.Password ?? string.Empty)                         // default demo user + your password
    .ServerCertificateValidationCallback(OpenSearch.Net.CertificateValidations.AllowAll) // for self-signed certs
    .DefaultIndex(appSettings?.OpenSearch?.AnimeIndex);                                            // optional default index
    // .DefaultMappingFor<Anime>(m => m.IndexName(appSettings?.OpenSearch?.AnimeIndex));                                            // optional default index
OpenSearchClient openSearchClient = new(settings);
builder.Services.AddSingleton<IOpenSearchClient>(openSearchClient);

builder.Services.AddScoped<IAnimeRepository, AnimeRepository>();
builder.Services.AddScoped<AnimeService>();

var app = builder.Build();


app.UseSerilogRequestLogging();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar", options =>
    {
        options.Theme = ScalarTheme.BluePlanet;
        options.AddDocument("v1", "Production API", "/openapi/{documentName}.json");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    app.MapGet("/", context =>
    {
        context.Response.Redirect("scalar");
        return Task.CompletedTask;
    });
}
else
{
    app.MapGet("/", () => $"${Ct.Common.AppName} is running");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Information($"{Ct.Common.AppName} start successfully");
Log.Information($"Run at environment: {environment}");

app.Run();

