using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using API.Helpers;
using API.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using API.Errors;
using StackExchange.Redis;
using Infrastructure.Identity;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddDbContext<StoreContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AppIdentityDbContext>(x =>
{
    x.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection"));
});
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
var b = builder.Services.AddIdentityCore<AppUser>();
b = new IdentityBuilder(b.UserType, b.Services);
b.AddEntityFrameworkStores<AppIdentityDbContext>();
b.AddSignInManager<SignInManager<AppUser>>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
         .Where(e => e.Value.Errors.Count > 0)
         .SelectMany(x => x.Value.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray();
        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors
        };
        return new BadRequestObjectResult(errorResponse);
    };
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);


        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.MigrateAsync();
        await AppIdentityDbContextSeed.SeedUserAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");

    }
}
// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseStatusCodePagesWithReExecute("/errors/{0}");



app.UseRouting();
app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod();
});
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
