bash <<'EOF'
set -e
echo "==> Registering MCP servers (filesystem, browser-tools, context7)"

# Filesystem - include project memory bank and MCP folder for quick access
claude mcp add filesystem -s user \
  -- npx -y @modelcontextprotocol/server-filesystem \
     ~/Documents ~/Desktop ~/Downloads ~/Projects \
     ./FocusModern/memory-bank ./MCP || true

# Browser-Tools - optional DevTools-powered browser utilities
claude mcp add browser-tools -s user \
  -- npx -y @agentdeskai/browser-tools-mcp || true

# Context7 - Upstash memory server (requires Upstash env vars)
if [ -z "$UPSTASH_REDIS_REST_URL" ] || [ -z "$UPSTASH_REDIS_REST_TOKEN" ] || \
   [ -z "$UPSTASH_VECTOR_REST_URL" ] || [ -z "$UPSTASH_VECTOR_REST_TOKEN" ]; then
  echo "WARN: Upstash env vars not set. See MCP/context7/README.md"
fi

claude mcp add context7 -s user \
  -- npx -y @upstash/context7 start || true

echo "-- MCP servers registered. You can verify with: claude mcp list"
EOF
