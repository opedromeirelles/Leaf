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

// Configuração da Autenticação de Cookies
builder.Services.AddAuthentication("CookieAuthentication")
        .AddCookie("CookieAuthentication", options =>
        {
            options.Cookie.Name = "UserLoginCookie"; // Nome do cookie
            options.LoginPath = "/Login";    // Caminho da página de login
            options.AccessDeniedPath = "/Login/ErrorLogin"; // Caminho de acesso negado
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Tempo de expiração do cookie
        });


// Adicionar minhas camadas de serviços

//Services Area:
builder.Services.AddTransient<UsuarioServices>();
builder.Services.AddTransient<DepartamentoServices>();
builder.Services.AddTransient<ProdutoServices>();
builder.Services.AddTransient<PessoaServices>();
builder.Services.AddTransient<InsumoServices>();
builder.Services.AddTransient<CompraServices>();
builder.Services.AddTransient<ItemCompraServices>();
builder.Services.AddTransient<PedidoServices>();
builder.Services.AddTransient<PedidoFacedeServices>();
builder.Services.AddTransient<ItemPedidoServices>();
builder.Services.AddTransient<LoteProducaoServices>();
builder.Services.AddTransient<ItemLoteProducaoServices>();


//Repositories:
builder.Services.AddTransient<UsuarioRepository>();
builder.Services.AddTransient<DepartamentoRepository>();
builder.Services.AddTransient<PessoaRepository>();
builder.Services.AddTransient<InsumoRepository>();
builder.Services.AddTransient<CompraRepository>();
builder.Services.AddTransient<ItemCompraRepository>();
builder.Services.AddTransient<PedidoRepository>();
builder.Services.AddTransient<ItemPedidoRepository>();
builder.Services.AddTransient<LoteProducaoRepository>();
builder.Services.AddTransient<ItemLoteProducaoRepository>();


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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
