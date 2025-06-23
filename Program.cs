using FilmesAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configura o contexto do banco de dados PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona suporte a controllers
builder.Services.AddControllers();

// Configura o Swagger com documenta��o personalizada
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "API de Filmes e S�ries",
        Version = "v1",
        Description = "API para cadastro e consulta de filmes e s�ries com persist�ncia em PostgreSQL.",
        Contact = new OpenApiContact {
            Name = "Renato Luis de Oliveira",
            Email = "prof.renato.oliveira2023@gmail.com"
        }
    });

    // Inclui os coment�rios XML na documenta��o do Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Filtro inline para adicionar servidores
    //c.DocumentFilter<AddServersFilter>();
});

var secretKey = Encoding.UTF8.GetBytes("UmAnelParaGovernar_Todos_123!EncontralosComSeguranca");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "FilmesAPI",
            ValidAudience = "FilmesAPI",
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };

        options.Events = new JwtBearerEvents {
            OnChallenge = context => {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new {
                    mensagem = "Token ausente ou inv�lido. � necess�rio autenticar-se."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });

// Corrige comportamento de datas com PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

// Middleware do Swagger
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Filmes e S�ries v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


// Classe do filtro inline
//public class AddServersFilter : IDocumentFilter {
//    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
//        swaggerDoc.Servers = new List<OpenApiServer>
//        {
//            new OpenApiServer { Url = "https://localhost:5196", Description = "Servidor de Desenvolvimento" },
//            new OpenApiServer { Url = "https://api.seudominio.com", Description = "Servidor de Produ��o" }
//        };
//    }
//}