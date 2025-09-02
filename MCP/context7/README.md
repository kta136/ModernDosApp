Context7 MCP Server Integration
================================

This repo integrates Upstash Context7 (an MCP server) to provide long-term memory for agents.

- Project: https://github.com/upstash/context7
- Purpose: persistent memory backed by Upstash Redis + Upstash Vector.

Setup
-----

1) Provision Upstash services
- Create an Upstash Redis database
- Create an Upstash Vector index
- Copy REST URLs and tokens for both services

2) Configure environment
- Copy `.env.example` to `.env`
- Set the following variables:

UPSTASH_REDIS_REST_URL="https://..."
UPSTASH_REDIS_REST_TOKEN="..."
UPSTASH_VECTOR_REST_URL="https://..."
UPSTASH_VECTOR_REST_TOKEN="..."

3) Register the MCP server
- Use the provided helper script to register Context7 with Claude MCP:

bash MCP/setup.sh

Alternatively, register manually:

claude mcp add context7 -s user \
  -- npx -y @upstash/context7 start

Make sure the environment variables above are exported in the same shell before running the command.

Files
-----

- MCP/setup.sh — helper script to add Context7 and filesystem pointers
- MCP/context7/.env.example — env var template; copy to `.env`

Notes
-----

- This repo excludes actual `.env` files from Git. Keep your tokens secret.
- The filesystem MCP server is also configured to expose `FocusModern/memory-bank` as a browsable path for the assistant.
