using AxiomSysDL.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Read the Oracle connection string from configuration.
var connectionString =
    builder.Configuration.GetConnectionString("SupplyChain")
    ?? throw new InvalidOperationException(
        "Connection string 'SupplyChain' is missing.");

// Register the Oracle database context.
builder.Services.AddDbContext<SupplyChainContext>(options =>
    options.UseOracle(connectionString));

// Add API services.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();