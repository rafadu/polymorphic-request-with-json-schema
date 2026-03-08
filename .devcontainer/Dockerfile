FROM mcr.microsoft.com/dotnet/sdk:10.0

# Instala Node.js (requisito do Claude Code)
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean

# Claude Code
RUN npm install -g @anthropic-ai/claude-code

WORKDIR /workspace