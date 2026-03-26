# ESTÁGIO 1: Build (Node.js)
FROM node:20-alpine AS build-stage
WORKDIR /app

# Copia os arquivos de dependências
COPY package*.json ./

# Instala as dependências (limpo e rápido)
RUN npm install

# Copia o restante do código fonte
COPY . .

# Gera o build de produção (gera a pasta /dist)
RUN npm run build

# ESTÁGIO 2: Produção (Nginx)
FROM nginx:stable-alpine AS production-stage

# Copia os arquivos estáticos do estágio de build para o diretório do Nginx
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Config nginx
COPY ./nginx.conf /etc/nginx/conf.d/default.conf

# Expõe a porta 80 (padrão do Nginx)
EXPOSE 80

# O Nginx inicia automaticamente, não precisa de ENTRYPOINT complexo
CMD ["nginx", "-g", "daemon off;"]