using Lab10_AlberthMayta.Application;
using Lab11_AlberthMayta.Configuration;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios ---
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// --- Pipeline (Middleware) ---
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
