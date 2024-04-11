using Servises.Interfaces;
using Servises.Services;
using Repositories;
using Models.Entities;
using Models.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Api.AuthorizationPolicy.IsBlocked;
using Api.AuthorizationPolicy.IsBunned;

namespace BookShop;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ShopContext>
            (options => options.UseNpgsql(connection, b => b.MigrationsAssembly("BookShop")));

        builder.Services.AddControllers();
        var provader = builder.Services.BuildServiceProvider();


        

        builder.Services.AddScoped<IAuthorizationHandler, IsBlockedHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, IsBunnedHandler>();

        builder.Services.AddScoped<Servises.Interfaces.IAuthorizationService, AuthorizationService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IAdministratorService, AdministratorService>();
        builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("IsNotBlocked", policy =>
                policy.Requirements.Add(new IsBlockedRequirement()));
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("IsNotBunned", policy =>
                policy.Requirements.Add(new IsBunnedRequirement()));
        });
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
                new string[] {}
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
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

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}


