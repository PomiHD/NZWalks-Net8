using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks.Data;
using NZWalks.Mappings;
using NZWalks.Models.DTO;
using NZWalks.Models.Repositories;
using PostmarkDotNet;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Warning().CreateLogger();

//clear default logging providers and add serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddControllers();

//add http context accessor to services collection to allow access to http context in controllers
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //add swagger doc for v1 of the API
    options.SwaggerDoc("v1", new() { Title = "NZWalks API", Version = "v1" });
    //add security definition to swagger doc for jwt bearer authentication scheme
    options.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
        }
    );
    //add security requirement to operations in swagger doc that require authentication and authorization
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    },
                    Scheme = "oauth2",
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        }
    );
});

//add db context
builder.Services.AddDbContext<NzWalksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString"))
);
builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString"))
);

//add difficulty repository
builder.Services.AddScoped<IRegionRepository, SqlRegionRepository>();

//add walk repository
builder.Services.AddScoped<IWalkRepository, SqlWalkRepository>();

//add token repository
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

//add image repository
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

//add automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

//add Identity services
builder
    .Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});
builder.Services.Configure<PostmarkSettings>(builder.Configuration.GetSection("Postmark"));

// Configure PostmarkClient with the server token from app settings
builder.Services.AddTransient<PostmarkClient>(provider => new PostmarkClient(
    builder.Configuration["Postmark:ServerToken"]
));

//add authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        }
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//add authentication middleware to the pipeline
app.UseAuthentication();

//add authorization middleware to the pipeline
app.UseAuthorization();

// add static files middleware to the pipeline
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, "Images")
        ),
        RequestPath = "/Images"
    }
);

app.MapControllers();

app.Run();
