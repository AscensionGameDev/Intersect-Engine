using Intersect.OpenPortChecker;

var builder = WebApplication.CreateBuilder(args);

var portCheckerOptionsSection = builder.Configuration.GetSection("PortChecker");
builder.Services.Configure<PortCheckerOptions>(portCheckerOptionsSection);

builder.Services.AddLogging();
builder.Services.AddSingleton<PortChecker>();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();