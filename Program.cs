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

// Configura o Swagger com documentação personalizada
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "API de Filmes e Séries",
        Version = "v1",
        Description = "API para cadastro e consulta de filmes e séries com persistência em PostgreSQL.",
        Contact = new OpenApiContact {
            Name = "Renato Luis de Oliveira",
            Email = "prof.renato.oliveira2023@gmail.com"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Esquema de segurança com tipo HTTP e esquema bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "Insira apenas o token JWT. O prefixo 'Bearer' será adicionado automaticamente.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var secret = builder.Configuration.GetValue<string>("SecretKey");
if (string.IsNullOrWhiteSpace(secret))
    throw new InvalidOperationException("A chave JwtSecret não foi encontrada no appsettings.json.");

var secretKey = Encoding.UTF8.GetBytes(secret);
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
                    mensagem = "Token ausente ou inválido. É necessário autenticar-se."
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Filmes e Séries v1");
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
//            new OpenApiServer { Url = "https://api.seudominio.com", Description = "Servidor de Produção" }
//        };
//    }
//}