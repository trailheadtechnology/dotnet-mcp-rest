#!/bin/bash
# Start the TodoMcp SSE Server for Claude Desktop
# This script starts the server in SSE mode (REST API + MCP over HTTP/SSE)

cd "$(dirname "$0")"

echo "🔍 Checking if server is already running on port 5555..."
if lsof -i:5555 -t > /dev/null 2>&1; then
    echo "⚠️  Port 5555 is already in use. Stopping existing process..."
    lsof -ti:5555 | xargs kill -9
    sleep 1
fi

echo "🚀 Starting TodoMcp SSE Server on port 5555..."
echo ""
dotnet run --project TodoAPI

# Note: Server will run in the foreground
# Press Ctrl+C to stop the server
