using whatsapp_clone_backend;

var builder = WebApplication.CreateBuilder(args);

// Read the connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("Default");

// Optional: Register the connection string globally in DI if needed
builder.Services.AddSingleton(new DbContext(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
