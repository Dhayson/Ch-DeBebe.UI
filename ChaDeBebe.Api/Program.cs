using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Porta padrão do Vite
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Autenticação JWT
// 1. Registrar o TokenService no container
builder.Services.AddScoped<TokenService>();

// 2. Configurar a Autenticação JWT
var chave = System.Text.Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]!);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Em desenvolvimento/Docker
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chave),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();


// Adicionar serviços de build
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 1. Cria o botão "Authorize" e define como o token deve ser passado
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Cabeçalho de autorização JWT usando o esquema Bearer. <br/>
                        Digite 'Bearer' [espaço] e em seguida o seu token. <br/>
                        Exemplo: <b>'Bearer eyJhbGci...'</b>",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Obriga o Swagger a enviar o token no cabeçalho de todas as requisições
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Adiciona o AddDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Usar o CORS no app
app.UseCors("WebAppPolicy");

// JWT
app.UseAuthentication();
app.UseAuthorization();

// Prototipação: criar rapidamente o banco de dados
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Cria o banco e as tabelas se não existirem
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adicionar router
app.MapAllEndpoints();

// Configura pasta de imagens
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "./app/upload")),
    RequestPath = "/app/upload" // Este será o prefixo na URL
});

app.Run();

// Essencial para o WebApplicationFactory encontrar a API
public partial class Program { }
