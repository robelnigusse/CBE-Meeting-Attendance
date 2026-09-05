FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

RUN npm run build

FROM nginx:stable-alpine

# Custom nginx config: SPA routing + reverse proxy /api -> backend container
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built files from fron node build
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]