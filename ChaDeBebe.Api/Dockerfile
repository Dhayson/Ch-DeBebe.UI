# ESTÁGIO 1: Compilação (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivos de projeto (.csproj)
COPY ["ChaDeBebe.Api/ChaDeBebe.Api.csproj", "ChaDeBebe.Api/"]

RUN dotnet restore "ChaDeBebe.Api/ChaDeBebe.Api.csproj"

# Copia o restante do código e compila
COPY . .
WORKDIR "/app/ChaDeBebe.Api"
RUN dotnet build "ChaDeBebe.Api.csproj" -c Release -o /app/build

# ESTÁGIO 2: Publicação (Publish)
FROM build AS publish
RUN dotnet publish "ChaDeBebe.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ESTÁGIO 3: Execução (Runtime - Imagem Final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
# Copia apenas os binários compilados do estágio anterior
COPY --from=publish /app/publish .

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "ChaDeBebe.Api.dll"]