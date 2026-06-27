using API.Middlewares;
using Asp.Versioning;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMediatR(opt =>
{
    opt.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA5NjQ4MDAwIiwiaWF0IjoiMTc3ODE3MDA3MiIsImFjY291bnRfaWQiOiIwMTllMDMyZDNjNjA3MzQ5YmRlMGU2MjZkMTI4MzY0OCIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa3IxazQzZzB6N2c3amI4MXk4cmR6cDM3Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.q3_sqZlT3DxFTDZ86k6pSxG29UEkvR_4zGABShmaet5En-R9m5Q78ZD9YT6QJ4CaNd618q1w6Ko6aod_U6qUtHYF9T1P73Y2ABKc63E4iBaAbSYU2YImNLeLlpNt-mRzd7e2HCXuCpR7TOEFHE0vP_joqwC-g1KJOHy-GWQa27na_F8yXNEqJKGtVnP-yOWL1U8AWjRe0XrtyVTPV-AU8wSGg8Frv_acbfZABZm9I3KJgWQXZOh6RfagPNF7hi0bf3gESOsbUQKo8Hry-BGAkzZmtBPUwEJL_DncoD54oIJpOGsGRtFhkXEf_sc68q9xWGSyGuuxDYuQYspz8GuCfw";
    opt.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'V";
    opt.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

app.UseMiddleware<GlobalException>();
app.UseMiddleware<IdempotencyMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
