using JWTAuthentication.Const;
using JWTAuthentication.Context;
using JWTAuthentication.Interfaces.Repos;
using JWTAuthentication.Interfaces.Services;
using JWTAuthentication.Middleware;
using JWTAuthentication.Repos;
using JWTAuthentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Load Config Data 
AuthConst.LoadConfigData();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1", Description = "Swagger for Auth system authorized by ZinKo" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,
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
                                }
                            },
                            new List<string>()
                        }
                    });
});

//builder.Services.AddDbContext<AuthContext>(options =>
//                            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AuthContext>(options =>
                              options.UseSqlServer(AuthConst.DB_CONNECTION));

builder.Services.AddScoped<IAuth, AuthRepository>();
builder.Services.AddScoped<IRoleAuthorizationService, RoleAuthenticationService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            //IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                            //                builder.Configuration.GetSection("AppSettings:SecrectKey").Value)),
                            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                                               AuthConst.TOKEN_SECRECT)),
                            ValidateIssuer = true,
                            //ValidIssuer = builder.Configuration.GetSection("AppSettings:Issuer").Value,
                            ValidIssuer = AuthConst.TOKEN_ISSUER,
                            ValidateAudience = false,
                        };
                    });

var app = builder.Build();

//Swagger Login
app.UseSwaggerAuthorization();

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
