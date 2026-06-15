using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Maintenance.Middleware;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Maintenance.Infrastructure.SqlServer.Repositories.Auth;
using Maintenance.UseCase.AuthUseCase;
using Equipment.Infrastructure.SqlServer.Repositories.Equipment;
using Maintenance.UseCase.EquipmentUseCase;
using Maintenance.UseCase.MaintenanceUseCase;
using Maintenance.Infrastructure.SqlServer.Repositories.Maintenance;
using Maintenance.Infrastructure.SqlServer.Repositories.Factory;
using Maintenance.UseCase.FactoryUseCase;
using Maintenance.UseCase.SyncDataService;
using Maintenance.Infrastructure.SqlServer.SyncData;
using SAPbobsCOM;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")
    ).LogTo(Console.WriteLine, LogLevel.Information)
    );


builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"] ?? "")),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(425);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddSingleton<Company>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();

    Console.WriteLine("========== SAP CONFIG ==========");
    Console.WriteLine($"Server      : {configuration["Connections:Sap:Server"]}");
    Console.WriteLine($"CompanyDB   : {configuration["Connections:Sap:Company"]}");
    Console.WriteLine($"UserName    : {configuration["Connections:Sap:UserName"]}");
    Console.WriteLine($"Password    : {configuration["Connections:Sap:Password"]}");
    Console.WriteLine($"DbServerType: {BoDataServerTypes.dst_MSSQL2014}");
    Console.WriteLine("================================");

    var company = new Company();

    company.Server = configuration["Connections:Sap:Server"];
    company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
    company.CompanyDB = configuration["Connections:Sap:Company"];
    company.UserName = configuration["Connections:Sap:UserName"];
    company.Password = configuration["Connections:Sap:Password"];
    company.language = SAPbobsCOM.BoSuppLangs.ln_English;
    company.UseTrusted = false;

    int result = company.Connect();

    if (result != 0)
    {
        company.GetLastError(out int errCode, out string errMsg);
        throw new Exception(
            $"SAP DI connect failed: {errCode} - {errMsg}"
        );
    }
    return company;
});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient();

//Add Service Repository
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISyncDataRepository, SyncDataRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IFactoryRepository, FactoryRepository>();


//Add Service UseCase
builder.Services.AddScoped<DataContextSql>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IFactoryService, FactoryService>();

//Add Service Job
builder.Services.AddTransient<SyncDataService>();

builder.Services.AddCors(
    option =>
    {
        option.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    }
);

builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("ApplicationDbContext")));
builder.Services.AddHangfireServer();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Maintenance Api", Version = "v0.0.1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = 104857600;
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<CustomAuthorizationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<VersionValidationMiddleware>();
//app.UseMiddleware<DeviceValidationMiddleware>();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.MapHangfireDashboard();
app.UseStaticFiles();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var syncDataJob = scope.ServiceProvider.GetRequiredService<SyncDataService>();
    syncDataJob.CreateSyncMachineJob();
    syncDataJob.CreateSyncEquipmentJob();
    syncDataJob.CreateSyncDataUserJob();
    syncDataJob.CreateMaintenancePeriodicJob();
    syncDataJob.CreateMaintenanceJob();
}

app.UseHangfireDashboard("/job-dashboard", new DashboardOptions
{
    DashboardTitle = "Maintenance Job Dashboard",
    DarkModeEnabled = false,
    DisplayStorageConnectionString = false,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = builder.Configuration["JobDashboard:username"],
            Pass = builder.Configuration["JobDashboard:password"]
        }
    },
});

app.Run();