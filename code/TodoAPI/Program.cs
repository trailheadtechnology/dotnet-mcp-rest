var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5555");

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MCP server with HTTP transport (for SSE)
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Swagger endpoints
app.UseSwagger();
app.UseSwaggerUI();

// MCP endpoints
app.MapMcp("/mcp");

// REST endpoints
app.MapEndpoints();

await app.RunAsync();