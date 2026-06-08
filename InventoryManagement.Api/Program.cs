using InventoryManagement.Api.Modules.Catalog.Data;
using InventoryManagement.Api.Modules.Catalog.Services.Category;
using InventoryManagement.Api.Modules.Catalog.Services.Product;
using InventoryManagement.Api.Modules.Sales.Data;
using InventoryManagement.Api.Modules.Sales.Services;
using InventoryManagement.Api.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/* CONTROLLERS */
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

/* SERVICES */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<SalesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

/* HTTP REQUEST PIPELINE */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
