using demoidp.Controllers;
using demoidp.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("DIDP_");
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<Settings>(builder.Configuration);

builder.Services.AddIdentityServer(options =>
    {
        options.UserInteraction.LoginUrl = "authenticate";
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;

        options.EmitStaticAudienceClaim = true;
    })
    .AddInMemoryApiResources(IdSrvConfig.GetApis())
    .AddInMemoryIdentityResources(IdSrvConfig.GetIdentityResources())
    .AddInMemoryClients(IdSrvConfig.GetClients())
    //.AddEmbeddedSigningCredential()
    .AddDeveloperSigningCredential(filename: Path.Combine(Path.GetTempPath(), "tempkey.jwk"));

builder.Services.AddTransient<ICorsPolicyService, DemoCorsPolicyService>();
builder.Services.AddTransient<IRedirectUriValidator, DemoRedirectValidator>();

var app = builder.Build();

app.UseSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles("/static");
app.UseIdentityServer();

app.Run();