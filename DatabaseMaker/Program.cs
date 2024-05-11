using DatabaseMaker.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
builder.Services.AddDbContext<Context>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Run();
