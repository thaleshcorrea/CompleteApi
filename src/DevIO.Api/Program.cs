using DevIO.Api.Configuration;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ResolveDependencies();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.WebApiConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //app.UseCors("Development");
}
else
{
    // Da um sinal para o browser, falando que ele usa somente HTTPS
    // Possui uma brecha, se a url for chamada usando somente HTTP, ele vai funcionar normalmente, não requerindo
    // Para isso usamos outra configuração, que é o UseHttpsRedirection, que mesmo enviando uma requisição http ele sempre vai redirecionar para HTTPS
    app.UseHsts();

    app.UseCors("Production");
}

app.UseAuthentication();

app.UseApiConfiguration();

app.MapControllers();

app.Run();
