using PayrollLedgerSync.Api.Endpoints;
using PayrollLedgerSync.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "ApiCorsPolicy";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiDependencies(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:4173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicyName);
app.UseHttpsRedirection();
app.MapLedgerSyncEndpoints();

app.Run();
