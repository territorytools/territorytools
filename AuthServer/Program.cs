
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
services.AddSingleton<RsaSecurityKey>(provider => {
    // It's required to register the RSA key with depedency injection.
    // If you don't do this, the RSA instance will be prematurely disposed.

    RSA rsa = RSA.Create();
    rsa.ImportRSAPublicKey(
        source: Convert.FromBase64String(@"MIIBCgKCAQEA1MROptDvMq8n4LiCUgIyF8xEqnXxIZYQC35mgEnpWf/FM1EkpIBC0ZSaIRAhWfr4GwlVGCCUL1jxPNPMXhvGRrjxr5Bmz0uv//F3zrfnmyJwkLd4SJC/DtE7798CBQExyB2AKu4PQoxsWz2Pyp89B/WOuWMOmvBgpLzU5YmgC3GZzYL+wAb8gE9uhOp315HUhV1akDe/HkxQv8kIVik2Tchq8JWzlXbHCTtKPKoVVfk/BiE8gZvB3uG2S4ZSaUGhBHlhfFoJagEceldc2seaffZDFA1F7F5pOAvaPgV5TbC5N97FoOs8pFoCFll3dmxOxRurPUW2ng0QALirM72KLQIDAQAB"),
        //source: Convert.FromBase64String(configuration["Jwt:Asymmetric:PublicKey"]),
        bytesRead: out int _
    );

    return new RsaSecurityKey(rsa);
});
services.AddAuthentication()
    .AddJwtBearer("Asymmetric", options => {
        SecurityKey rsa = services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();

        options.IncludeErrorDetails = true; // <- great for debugging

                    // Configure the actual Bearer validation
                    options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = rsa,
            ValidAudience = "jwt-test",
            ValidIssuer = "jwt-test",
            RequireSignedTokens = true,
            RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
                        ValidateLifetime = true, // <- the "exp" will be validated
                        ValidateAudience = true,
            ValidateIssuer = true,
            
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
