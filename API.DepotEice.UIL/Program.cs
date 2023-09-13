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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using System.Text;

namespace API.DepotEice.UIL;

/// <summary>
/// App's entry class
/// </summary>
public class Program
{
    /// <summary>
    /// App's entry point
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <exception cref="NullReferenceException"></exception>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureAppConfiguration((hostContext, webBuilder) =>
        {
            if (hostContext.HostingEnvironment.IsDevelopment())
            {
                webBuilder.AddUserSecrets<Program>();
            }
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000);
        });

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

            string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

#if DEBUG
        string jwtSecret = builder.Configuration["JWT:JWT_SECRET"] ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_SECRET\"");

        string jwtIssuer = builder.Configuration["JWT:JWT_ISSUER"] ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_ISSUER\"");

        string jwtAudience = builder.Configuration["JWT:JWT_AUDIENCE"] ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable with name " +
                $"\"JWT_AUDIENCE\"");
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
#endif

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
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

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
        string connectionString = builder.Configuration["DATASOURCE:MSSQL_CONNECTION_STRING"] ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named " +
                $"MSSQL_CONNECTION_STRING");
#else
        string connectionString = Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING") ??
            throw new NullReferenceException($"{DateTime.Now} - There is no environment variable named " +
                $"MSSQL_CONNECTION_STRING");
#endif
        builder.Services.AddSingleton<IDevHopConnection, MsSqlCon>(o => new MsSqlCon(connectionString));

        builder.Services.AddSingleton<ITokenManager, TokenManager>();

        /******************/
        /*  Repositories  */
        /******************/

        builder.Services.AddScoped<IAddressRepository, AddressRepository>();
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
        builder.Services.AddScoped<IFileRepository, FileRepository>();

        builder.Services.AddTransient<IDateTimeManager, DateTimeManager>();
        builder.Services.AddTransient<IUserManager, UserManager>();
        builder.Services.AddTransient<IFileManager, FileManager>();

        builder.Services.AddSingleton<ChatManager>();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Depot.Eice v1"));

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseStaticFiles();

        // TODO: Remove this and add an exception
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/hub/chat");

        app.Run();
    }
}