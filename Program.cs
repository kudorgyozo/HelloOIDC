using HelloOIDC.Auth;
using HelloOIDC.Repo;
using Microsoft.AspNetCore.Authorization;
using SQLite;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options => {
    options.AddPolicy(MyAllowSpecificOrigins,
        policy => {
            policy.WithOrigins("http://localhost:3000");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowCredentials();
        });
});
builder.Services.AddControllers();
builder.Services.AddSingleton<OIDCRepo>();
builder.Services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();

builder.Services.AddAuthentication()
    .AddScheme<RoleAuthenticationOpts, RoleAuthenticationHandler>(
        RoleAuthenticationHandler.SchemeName,
        opts => { }
    );
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
