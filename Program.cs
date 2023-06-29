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
    options.AddPolicy(name: MyAllowSpecificOrigins, policy  => { policy.WithOrigins("http://localhost:8002").AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IProductsService, MongoDbProductsService>();
builder.Services.AddSingleton<IProductCategoryService, MongoDbProductCategoryService>();
builder.Services.AddSingleton<ITransactionService, MongoTransactionService>();
builder.Services.AddSingleton<IInventoryManager, DefaultInventoryManager>();
builder.Services.AddSingleton<IStockService, MongoStockService>();
builder.Services.AddSingleton<IPurchasesService, MongoPurchasesService>();
builder.Services.AddSingleton<ISupplierService, MongoSupplierService>();

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var settings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
