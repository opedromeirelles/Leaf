using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Leaf.Data;
using Leaf.Repository;
using Leaf.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexão com banco
builder.Services.AddSingleton<DbConnectionManager>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DbConnectionManager(connectionString);
});

// Adicionar minhas camadas de serviços

//Services Area:
builder.Services.AddTransient<UsuarioServices>();
builder.Services.AddTransient<DepartamentoServices>();
builder.Services.AddTransient<ProdutoServices>();
builder.Services.AddTransient<PessoaServices>();
builder.Services.AddTransient<InsumoServices>();




//Repositories:
builder.Services.AddTransient<UsuarioRepository>();
builder.Services.AddTransient<DepartamentoRepository>();
builder.Services.AddTransient<PessoaRepository>();
builder.Services.AddTransient<InsumoRepository>();



// Configurar a cultura padrão
var defaultCulture = new CultureInfo("pt-BR");
var supportedCultures = new[] { defaultCulture };

// Configuração de serviços de localização
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Habilitar a localização com a cultura configurada
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
