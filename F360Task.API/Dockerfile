# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["F360Task.API/F360Task.API.csproj", "F360Task.API/"]
COPY ["F360Task.Application/F360Task.Application.csproj", "F360Task.Application/"]
COPY ["F360Task.Infrastructure/F360Task.Infrastructure.csproj", "F360Task.Infrastructure/"]
COPY ["F360Task.Domain/F360Task.Domain.csproj", "F360Task.Domain/"]
COPY ["F360Task.EventBusRabbitMQ/F360Task.EventBusRabbitMQ.csproj", "F360Task.EventBusRabbitMQ/"]
COPY ["F360Task.EventBus/F360Task.EventBus.csproj", "F360Task.EventBus/"]
COPY ["F360Task.Report.Infrastructure/F360Task.Report.Infrastructure.csproj", "F360Task.Report.Infrastructure/"]
COPY ["F360Task.Report.Domain/F360Task.Report.Domain.csproj", "F360Task.Report.Domain/"]
COPY ["F360Task.SharedKernel/F360Task.SharedKernel.csproj", "F360Task.SharedKernel/"]
RUN dotnet restore "./F360Task.API/F360Task.API.csproj"
COPY . .
WORKDIR "/src/F360Task.API"
RUN dotnet build "./F360Task.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./F360Task.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "F360Task.API.dll"]