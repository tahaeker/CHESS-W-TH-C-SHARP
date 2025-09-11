using ChessWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ChessService'i dependency injection’a ekle
builder.Services.AddSingleton<ChessService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
