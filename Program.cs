using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using whatsapp_clone_backend;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Read JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Register DbContext with Scoped lifetime
builder.Services.AddScoped<DbContext>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    return new DbContext(connectionString);
});

// Register DL class
builder.Services.AddScoped<Login_DL>();
builder.Services.AddScoped<Message_DL>();

builder.Services.AddScoped<SearchUser_DL>();

builder.Services.AddScoped<Registration_DL>();
builder.Services.AddScoped<EmailService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddMemoryCache();



// Add JWT Authentication (optional now, but needed soon)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 2147483647; // 2 GB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 2147483647; // 2 GB
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://whatsapp-react-c3s9-h7p8d7y1j-rayyan2907s-projects.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Only needed if you're using cookies/auth headers
    });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
    

// Swagger in dev
if (app.Environment.IsDevelopment())
{
}

app.UseCors("AllowFrontend");



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
