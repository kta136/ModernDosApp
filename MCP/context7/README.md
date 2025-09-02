Context7 MCP Server Integration
================================

This repo integrates Upstash Context7 (an MCP server) to provide long-term memory for agents.

- Project: https://github.com/upstash/context7
- Purpose: persistent memory via a single Context7 API key.

Setup
-----

1) Configure environment
- Copy `MCP/context7/.env.example` to `MCP/context7/.env`
- Set the following variable:

CONTEXT7_API_KEY="ctx7sk-..."

2) Register the MCP server
- Use the helper script to register Context7 with Claude MCP:

bash MCP/setup.sh

Alternatively, register manually (ensure env var is exported):

CONTEXT7_API_KEY="ctx7sk-..." \
claude mcp add context7 -s user -- npx -y @upstash/context7 start

Files
-----

- MCP/setup.sh — helper script to add Context7 and filesystem pointers
- MCP/context7/.env.example — env var template; copy to `.env`

Notes
-----

- `.env` files are ignored by Git. Keep your key secret.
- The filesystem MCP server is also configured to expose `FocusModern/memory-bank` as a browsable path for the assistant.
