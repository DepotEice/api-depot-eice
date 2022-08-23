using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Services;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Repositories;
using API.DepotEice.UIL.Hubs;
using API.DepotEice.UIL.IManagers;
using API.DepotEice.UIL.Managers;
using DevHopTools.Connection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace API.DepotEice.UIL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors();
            builder.Services.AddControllers();
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

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsConnected", policy => policy.RequireAuthenticatedUser());
            });

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
            //string connectionString = builder.Configuration.GetConnectionString("LocalAspirio");
            string connectionString = builder.Configuration.GetConnectionString("LocalCrysis90war");
#else
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
#endif

            builder.Services.AddSingleton(sp => new Connection(connectionString, SqlClientFactory.Instance));

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

            /**************/
            /*  Services  */
            /**************/

            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IArticleCommentService, ArticleCommentService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IModuleService, ModuleService>();
            builder.Services.AddScoped<IOpeningHoursService, OpeningHoursService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IScheduleFileService, ScheduleFileService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserTokenService, UserTokenService>();



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Depot.Eice v1"));
            }

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
}