using KutuphaneCore.Entities;
using KutuphaneDataAccess;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.Helpers.ZLog;
using kutuphaneServis.Interfaces;
using kutuphaneServis.MapProfile;
using kutuphaneServis.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.RateLimiting;


//log yapýlandýrmasý
Log.Logger =  new LoggerConfiguration()
   .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(); //Serilog'u kullan


//kendi log sýnýfýmýzý kullanmak istersek
builder.Services.AddScoped<IZLogger, ZLogger>();

//RateLimiting yapýlandýrmasý
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter( "ReateLimiter", options =>
    {
        options.PermitLimit = 5; // Belirli bir zaman diliminde izin verilen maksimum istek sayýsý
        options.Window = TimeSpan.FromSeconds(10); // Zaman dilimi (örneðin, 10 saniye)
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst; // Kuyruk iþleme sýrasý
        options.QueueLimit = 2; // Kuyrukta bekleyebilecek maksimum istek sayýsý

    });

    options.AddFixedWindowLimiter("ReateLimiter2", options =>
    {
        options.PermitLimit = 5; // Belirli bir zaman diliminde izin verilen maksimum istek sayýsý
        options.Window = TimeSpan.FromSeconds(10); // Zaman dilimi (örneðin, 10 saniye)
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst; // Kuyruk iþleme sýrasý
        //options.QueueLimit = 2; // Kuyrukta bekleyebilecek maksimum istek sayýsý

    });

});


//JWT yapýlandýrmasý
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
    ).AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var isReadFromJwt = builder.Configuration.GetSection("Jwt:Key");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//builder.Services.AddSwaggerGen();
//swagger yapýlandýrmasýný deðiþtiriyoruz
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Udemy Kütüphane API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Lütfen 'Bearer' kelimesini ve ardýndan boþluk ile birlikte geçerli JWT tokenini girin.",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



//builder.Services.AddDbContext<DatabaseConnection>();

builder.Services.AddDbContext<DatabaseConnection>(optionsAction =>
{
    optionsAction.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//Servis implamantasyonu yaptýk
//AddTransient: her istekte yeni bir instance oluþturur.
//AddScoped : her istekte bir instance oluþturur ve isteðin sonunda yok eder.
builder.Services.AddScoped<IGenericRepository<Author>, Repository<Author>>();
builder.Services.AddScoped<IGenericRepository<Book>, Repository<Book>>();
builder.Services.AddScoped<IGenericRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IGenericRepository<User>, Repository<User>>();

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IAuthorServiceSP, AuthorServiceSp>();


builder.Services.AddAutoMapper(typeof(MapProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//RateLimiting middleware ekliyoruz
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
