# Build Stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY frontend/package*.json ./
RUN npm ci

# Copy source and build
COPY frontend/ .
# We accept VITE_API_URL as a build arg or env var. 
# Note: Vite embeds env vars at build time. 
# For Render, we might need to trick it or use a runtime solution, 
# but for simplicity, we assume RENDER_EXTERNAL_URL of backend is passed or hardcoded.
# Actually, better approach for runtime config is normally needed, 
# but for this university task, we will try to rely on environment variables present during build if possible,
# or user might need to rebuild if URL changes.
# Render allows setting env vars at build time.
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

RUN npm run build

# Serve Stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
