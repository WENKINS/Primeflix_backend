using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Primeflix.Data;
using Primeflix.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc()
    .AddNewtonsoftJson( o => o.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICelebrityRepository, CelebrityRepository>();
builder.Services.AddScoped<IFormatRepository, FormatRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IGenreTranslationRepository, GenreTranslationRepository>();
builder.Services.AddScoped<IProductTranslationRepository, ProductTranslationRepository>();
builder.Services.AddEntityFrameworkMySQL().AddDbContext<DatabaseContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DBConnection"));
});

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

app.UseMvc();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}"
        );
});
