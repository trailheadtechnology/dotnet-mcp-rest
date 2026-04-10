var builder = WebApplication.CreateBuilder(args);

// Remove console logging to keep stdout clean for stdio transport
// (HTTP transport doesn't need stdout, so this is safe)
builder.Logging.ClearProviders();
builder.Logging.AddDebug();

builder.WebHost.UseUrls("http://localhost:5555");

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MCP server with HTTP transport
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Swagger endpoints
app.UseSwagger();
app.UseSwaggerUI();

// MCP HTTP endpoint
app.MapMcp("/mcp");

// REST endpoints
app.MapEndpoints();

await app.RunAsync();