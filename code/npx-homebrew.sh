#!/bin/bash
# Wrapper to ensure Homebrew's Node.js (v24.9.0) is used instead of system Node.js (v18.14.0)
export PATH="/opt/homebrew/bin:$PATH"
exec /opt/homebrew/bin/npx "$@"
