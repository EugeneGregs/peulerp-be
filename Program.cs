using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PeyulErp.Services;
using PeyulErp.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.String));

//Cors
builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins, policy  => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

// configure Services
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton<IProductsService, MongoDbProductsService>();
    builder.Services.AddSingleton<IProductCategoryService, MongoDbProductCategoryService>();
    builder.Services.AddSingleton<ITransactionService, MongoTransactionService>();
    builder.Services.AddSingleton<IInventoryManager, DefaultInventoryManager>();
    builder.Services.AddSingleton<IStockService, MongoStockService>();
    builder.Services.AddSingleton<IPurchasesService, MongoPurchasesService>();
    builder.Services.AddSingleton<ISupplierService, MongoSupplierService>();
    builder.Services.AddSingleton<IExpenseService, MongoExpenseService>();
    builder.Services.AddSingleton<IDashboardService, DefaultDashboardService>();

    builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
    {
        var settings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        Console.WriteLine($"Connection String: {settings.ConnectionString}, Database Name: {settings.DatabaseName}");
        return new MongoClient(settings.ConnectionString);
    });


    builder.Services.Configure<MongoDbSettings>(
        builder.Configuration.GetSection("MongoDbSettings"));
    builder.Services.Configure<SystemSettings>(
        builder.Configuration.GetRequiredSection("SystemSettings"));

    builder.Services
    .AddMvc(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });
}

//Configuration 
{
    //KeyVault
    //string kvUri = builder.Configuration.GetSection("KeyVaultSettings").GetValue<String>("KvUrl");
    //string tenantId = builder.Configuration.GetSection("KeyVaultSettings").GetValue<String>("TeanantId");
    //string clientId = builder.Configuration.GetSection("KeyVaultSettings").GetValue<String>("ClientId");
    //string clientSecret = builder.Configuration.GetSection("KeyVaultSettings").GetValue<String>("ClientSecretId");

    //var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    //var client = new SecretClient(new Uri(kvUri), credential);
    //builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
