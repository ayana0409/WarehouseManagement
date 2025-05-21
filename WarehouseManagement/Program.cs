using Microsoft.EntityFrameworkCore;
using WarehouseManagement;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Repository;
using WarehouseManagement.Share;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//builder.WebHost.UseKestrel().UseUrls("http://0.0.0.0:5000"); // Chạy trên tất cả IP
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
        //    builder.AllowAnyOrigin()
        //           .AllowAnyMethod()
        //           .AllowAnyHeader();
            builder.WithOrigins("http://localhost:3000", "http://26.139.159.129", "http://26.139.159.129:3000", "http://localhost:3001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
}
    );
});
var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();



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
