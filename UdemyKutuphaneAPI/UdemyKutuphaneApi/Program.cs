using Katmanli.DataAccess.Entities;
using KutuphaneCore.Entities;
using KutuphaneDataAccess;
using KutuphaneDataAccess.Repository;
using kutuphaneServis.AI;
using kutuphaneServis.Helpers.ZLog;
using kutuphaneServis.Interfaces;
using kutuphaneServis.MapProfile;
using kutuphaneServis.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Security.Claims;
using System.Threading.RateLimiting;


//log yapýlandýrmasý
Log.Logger =  new LoggerConfiguration()
   .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog(); //Serilog'u kullan


//kendi log sýnýfýmýzý kullanmak istersek
builder.Services.AddScoped<IZLogger, ZLoggerService>();

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

var configuration = builder.Configuration;
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IAIService, OpenAiService>(c =>
{
    c.BaseAddress = new Uri("https://api.openai.com/v1/");
    var key = configuration["AI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key bulunamadý.");
    c.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
});
// OpenLibrary Provider kaydý
builder.Services.AddHttpClient<IExternalBookProvider, OpenLibraryProvider>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(10);
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
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            NameClaimType= ClaimTypes.Name,
            RoleClaimType = "Roles"
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


//cors ekliyoruz
var NgClient = "_ngClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: NgClient, policy =>
    {
        policy
        .WithOrigins("http://localhost:4200", "https://localhost:4200") // Angular dev server
            .AllowAnyHeader()
            .AllowAnyMethod();

    });
}
    );

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
builder.Services.AddScoped<IGenericRepository<Role>, Repository<Role>>();
builder.Services.AddScoped<IGenericRepository<UserRole>, Repository<UserRole>>();
builder.Services.AddScoped<IGenericRepository<UploadImage>, Repository<UploadImage>>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUploadImageService, UploadImageService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();



builder.Services.AddScoped<IAuthorServiceSP, AuthorServiceSp>();


builder.Services.AddAutoMapper(typeof(MapProfile));

var app = builder.Build();


/* ===== Bootstrap: Admin/User rol ve ilk admin ===== */
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var config = sp.GetRequiredService<IConfiguration>();
    var log = sp.GetRequiredService<IZLogger>();

    var userRepo = sp.GetRequiredService<IGenericRepository<User>>();
    var roleRepo = sp.GetRequiredService<IGenericRepository<Role>>();
    var urRepo = sp.GetRequiredService<IGenericRepository<UserRole>>();

    var email = config["InitialAdmin:Email"];
    var password = config["InitialAdmin:Password"]; // opsiyonel

    if (!string.IsNullOrWhiteSpace(email))
    {
        // 1) Rolleri garanti et
        var adminRole = roleRepo.GetAll().FirstOrDefault(r => r.NormalizedName == "ADMIN");
        if (adminRole == null)
        {
            adminRole = new Role { Name = "Admin", NormalizedName = "ADMIN" };
            roleRepo.Create(adminRole);
            log.Info("Bootstrap: 'Admin' rolü oluþturuldu.");
        }

        var userRole = roleRepo.GetAll().FirstOrDefault(r => r.NormalizedName == "USER");
        if (userRole == null)
        {
            userRole = new Role { Name = "User", NormalizedName = "USER" };
            roleRepo.Create(userRole);
            log.Info("Bootstrap: 'User' rolü oluþturuldu.");
        }

        // 2) Admin kullanýcýsýný bul / gerekirse oluþtur
        var adminUser = userRepo.GetAll().FirstOrDefault(u => u.Email == email);

        if (adminUser == null && !string.IsNullOrWhiteSpace(password))
        {
            adminUser = new User
            {
                Name = "Admin",
                Surname = "User",
                Username = "admin",
                Email = email,
                Password = HashLikeUserService(password), // UserService'teki ile AYNI algoritma
                RecordDate = DateTime.Now,
            };
            userRepo.Create(adminUser);
            log.Info($"Bootstrap: {email} kullanýcýsý oluþturuldu.");
        }

        // 3) Role linklerini garanti et (User + Admin)
        if (adminUser != null)
        {
            bool hasUser = urRepo.GetAll().Any(x => x.UserId == adminUser.Id && x.RoleId == userRole.Id);
            bool hasAdmin = urRepo.GetAll().Any(x => x.UserId == adminUser.Id && x.RoleId == adminRole.Id);

            if (!hasUser)
            {
                urRepo.Create(new UserRole { UserId = adminUser.Id, RoleId = userRole.Id });
                log.Info($"Bootstrap: {email} -> USER rolü atandý.");
            }
            if (!hasAdmin)
            {
                urRepo.Create(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                log.Info($"Bootstrap: {email} -> ADMIN rolü atandý.");
            }
        }
        else if (adminUser == null && string.IsNullOrWhiteSpace(password))
        {
            log.Warn("Bootstrap: InitialAdmin.Email var; kullanýcý yok ve Password verilmediði için oluþturulamadý.");
        }
    }
}
/* ===== Bootstrap sonu ===== */

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

    app.UseHttpsRedirection();

//RateLimiting middleware ekliyoruz
app.UseRateLimiter();

app.UseCors(NgClient);
//bunu koyduk
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


static string HashLikeUserService(string password)
{
    // UserService.HashedPassword ile ayný olmalý
    const string secretKey = "uKI7A:h=&AOv6IX4&[vPgr2:Mu<+Rh";
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + secretKey));
    return Convert.ToBase64String(bytes);
}