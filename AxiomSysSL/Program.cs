
using System.Text;
using System.Threading.RateLimiting;
using AxiomSysDL.Contexts;
using AxiomSysSL.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// Configuration
// =====================================

var connectionString =
    builder.Configuration.GetConnectionString("SupplyChain")
    ?? throw new InvalidOperationException(
        "Connection string 'SupplyChain' is missing.");

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is missing from user secrets.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must contain at least 32 bytes.");
}

var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer is missing.");

var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience is missing.");

var company = builder.Configuration["Assessment:CompanyCode"]
    ?? throw new InvalidOperationException(
        "Assessment:CompanyCode is missing.");


// =====================================
// Oracle Database
// =====================================

builder.Services.AddDbContext<SupplyChainContext>(
    options => options.UseOracle(connectionString));


// =====================================
// Business Services
// =====================================

builder.Services.AddSalesModule();

builder.Services.AddControllers();


// =====================================
// JWT Authentication
// =====================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = issuer,
                ValidAudience = audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                NameClaimType = "name",
                ClockSkew = TimeSpan.Zero
            };
    });


// =====================================
// Authorization
// =====================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CompanyScope", policy =>
        policy
            .RequireAuthenticatedUser()
            .RequireClaim("CompanyCode", company));
});


// =====================================
// Rate Limiting
// =====================================

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("login", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});


// =====================================
// Swagger + JWT Authorize Button
// =====================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Sales", new OpenApiInfo
    {
        Title = "Sales API - Delivery Terms",
        Version = "v1"
    });

    options.DocInclusionPredicate(
        (name, description) =>
            description.GroupName == name);

    // JWT Bearer Security Definition
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

    // Apply JWT Authentication to Swagger requests
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer", document)] = []
        });
});


// =====================================
// Build Application
// =====================================

var app = builder.Build();


// =====================================
// Swagger - Development Only
// =====================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/Sales/swagger.json",
            "Sales");
    });
}


// =====================================
// Middleware
// =====================================

app.UseHttpsRedirection();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();


// =====================================
// Map Controllers
// =====================================

app.MapControllers();

app.Run();
