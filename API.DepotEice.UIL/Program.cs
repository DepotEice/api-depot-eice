using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Repositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Hubs;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Managers;
using DevHopTools.DataAccess.Connections;
using DevHopTools.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using System.Text;

namespace API.DepotEice.UIL;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSignalR();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer { token }\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Depot Eice API",
                Description = "An ASP.NET Core Web API for managing DepotEice.",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // Retrieving JWT Token data from environment
#if DEBUG
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["AppSettings:Audience"]
            };
        });
#else
        string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_SECRET\"");
        string jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_ISSUER\"");
        string jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_AUDIENCE\"");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience
            };
        });
#endif
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("IsConnected", policy => policy.RequireAuthenticatedUser());
        });

        builder.Services.AddSingleton<IAuthorizationPolicyProvider, HasRolePolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, HasRoleRequirementHandler>();


        /****************/
        /*  AutoMapper  */
        /****************/

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        /*************/
        /*  Logging  */
        /*************/

        builder.Services.AddLogging();

        /****************/
        /*  Singletons  */
        /****************/

#if DEBUG
        string connectionString = builder.Configuration.GetConnectionString("LocalAspirio");
        //string connectionString = builder.Configuration.GetConnectionString("LocalCrysis90war");
#else
        string connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named " +
                $"MSSQL_CONNECTION_STRING");
#endif
        builder.Services.AddSingleton<IDevHopConnection, MsSqlCon>(o => new MsSqlCon(connectionString));
        // builder.Services.AddSingleton(sp => new MsSqlCon(connectionString));

        builder.Services.AddSingleton<ITokenManager>(new TokenManager(builder));

        /******************/
        /*  Repositories  */
        /******************/

        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        builder.Services.AddScoped<IArticleCommentRepository, ArticleCommentRepository>();
        builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
        builder.Services.AddScoped<IOpeningHoursRepository, OpeningHoursRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<IScheduleFileRepository, ScheduleFileRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();

        builder.Services.AddTransient<IDateTimeManager, DateTimeManager>();
        builder.Services.AddTransient<IUserManager, UserManager>();

        var app = builder.Build();

        //if (app.Environment.IsDevelopment())
        //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Depot.Eice v1"));
        //}

        app.UseStaticFiles();

        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseRouting();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/chat-hub");

        app.Run();
    }
}