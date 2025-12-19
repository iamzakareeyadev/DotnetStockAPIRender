using System.Text;
using DotnetStockAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    // options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")); //local
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionRender"));
});

// Adding Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options  => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection("JWT:ValidAudience").Value!,
        ValidIssuer = builder.Configuration.GetSection("JWT:ValidIssuer").Value!,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value!))
    };
});

builder.Services.AddCors(options => 
{
    options.AddPolicy("MultipleOrigins",
    policy =>
    {
        policy.WithOrigins(
            // "*" // Allow all origins
            "https://itgenius.co.th", // Allow specific origin
            "https://*.itgenius.co.th/", // Allow subdomain
            "https://*.azurewebsites.net/", // Azure Apps
            "https://*.netlify.app/", // Netlify Apps
            "https://*.vercel.app/", // Vercel Apps
            "https://*.herokuapp.com/", // Heroku Apps
            "https://*.firebaseapp.com/", // Firebase Apps
            "https://*.github.io/", // Github Pages
            "https://*.gitlab.io/", // Gitlab Pages
            "https://*.onrender.com/", // Render Apps
            "https://*.surge.sh/", // Surge Apps
            "http://localhost:8080", // Vue , Svelte Apps
            "http://localhost:4200", // Angular Apps
            "http://localhost:3000", // React Apps
            "http://localhost:5173", // Vite Apps
            "http://localhost:5000", // Blazor Apps
            "http://localhost:5001" // Blazor Apps
        )
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyMethod()
        .AllowAnyHeader();
        // Allow specific headers
        // .WithHeaders("Content-Type", "Authorization")
        // Allow specific methods
        // .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SupportNonNullableReferenceTypes();
        options.SwaggerDoc("v1", new() { Title = "StockAPI", Version = "v1" });

        options.AddSecurityDefinition("Bearer",  new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat= "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }
);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("MultipleOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
