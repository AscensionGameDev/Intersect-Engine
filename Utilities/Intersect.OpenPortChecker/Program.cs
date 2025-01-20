using System.Net;
using Intersect.OpenPortChecker;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var portCheckerOptionsSection = builder.Configuration.GetSection("PortChecker");
var portCheckerOptions = portCheckerOptionsSection.Get<PortCheckerOptions>();
builder.Services.Configure<PortCheckerOptions>(portCheckerOptionsSection);
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());

var startupLogger = loggerFactory.CreateLogger<Program>();

var knownProxies = portCheckerOptions?.KnownProxies;

if (knownProxies?.Any() ?? false)
{
    builder.Services.Configure<ForwardedHeadersOptions>(
        options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                       ForwardedHeaders.XForwardedProto |
                                       ForwardedHeaders.XForwardedHost;

            foreach (var rawProxyAddress in knownProxies)
            {
                if (string.IsNullOrWhiteSpace(rawProxyAddress))
                {
                    startupLogger.LogDebug("Invalid address '{RawProxyAddress}'", rawProxyAddress);
                }
                else if (IPAddress.TryParse(rawProxyAddress.Trim(), out var proxyAddress))
                {
                    startupLogger.LogDebug(
                        "Added {ProxyAddress} as a known good proxy for forwarded headers",
                        proxyAddress
                    );
                    options.KnownProxies.Add(proxyAddress);
                }
                else
                {
                    try
                    {
                        var addresses = Dns.GetHostAddresses(rawProxyAddress.Trim());
                        foreach (var address in addresses)
                        {
                            startupLogger.LogDebug(
                                "Added {ProxyAddress} as a known good proxy for forwarded headers",
                                address
                            );
                            options.KnownProxies.Add(address);
                        }

                        if (addresses.Length < 1)
                        {
                            startupLogger.LogWarning(
                                "Failed to resolve {RawProxyAddress}, unable to add as known proxy",
                                rawProxyAddress.Trim()
                            );
                        }
                    }
                    catch (Exception exception)
                    {
                        startupLogger.LogDebug(
                            exception,
                            "Failed to resolve {RawProxyAddress}",
                            rawProxyAddress.Trim()
                        );
                    }
                }
            }
        }
    );
}

builder.Services.AddLogging();
builder.Services.AddSingleton<PortChecker>();
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (knownProxies?.Any() ?? false)
{
    app.UseForwardedHeaders();
}

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