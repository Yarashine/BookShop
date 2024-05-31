using Servises.Interfaces;
using Servises.Services;
using Repositories;
using Models.Exceptions;
using Models.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Api.AuthorizationPolicy.IsBlocked;
using Models.Configs;
using Api.Middlewares;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace BookShop;
public class Program
{
    [Obsolete]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ShopContext>
            (options => options.UseNpgsql(connection, b => b.MigrationsAssembly("BookShop")));

        builder.Services.AddControllers();


        builder.Services.AddFluentValidation(v => v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

        builder.Services.AddScoped<IAuthorizationHandler, IsBlockedHandler>();

        builder.Services.AddScoped<Servises.Interfaces.IAuthorizationService, AuthorizationService>();
        builder.Services.AddScoped<ITokenService, Servises.Services.TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IAdministratorService, AdministratorService>();
        builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        builder.Services.Configure<StripeSessionConfig>(opt => builder.Configuration.GetSection("StripeOptions").Bind(opt));
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("IsNotBlocked", policy =>
                policy.Requirements.Add(new IsBlockedRequirement()));
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
                }
        });
        });

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key" ] 
                ?? throw new BadRequestException("key is null"))),
                ClockSkew = TimeSpan.Zero
            };
        });



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();


        //app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}


