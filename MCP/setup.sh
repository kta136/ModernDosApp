bash <<'EOF'
set -e
echo "==> Registering MCP servers (filesystem, browser-tools, context7)"

# Load local env if present (Context7 key, etc.)
if [ -f MCP/context7/.env ]; then
  set -a
  . MCP/context7/.env
  set +a
fi

# Filesystem - include project memory bank and MCP folder for quick access
claude mcp add filesystem -s user \
  -- npx -y @modelcontextprotocol/server-filesystem \
     ~/Documents ~/Desktop ~/Downloads ~/Projects \
     ./FocusModern/memory-bank ./MCP || true

# Browser-Tools - optional DevTools-powered browser utilities
claude mcp add browser-tools -s user \
  -- npx -y @agentdeskai/browser-tools-mcp || true

# Context7 - Upstash memory server (requires Upstash env vars)
if [ -z "$CONTEXT7_API_KEY" ]; then
  echo "WARN: CONTEXT7_API_KEY not set. Put it in MCP/context7/.env"
fi

claude mcp add context7 -s user \
  -- npx -y @upstash/context7 start || true

echo "-- MCP servers registered. You can verify with: claude mcp list"
EOF
