# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файлы решения и проектов
COPY *.sln .
COPY WebApplication2/*.csproj ./WebApplication2/
COPY WebApplication2/*.config ./WebApplication2/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем весь исходный код
COPY . .

# Собираем и публикуем
WORKDIR "/src/WebApplication2"
RUN dotnet publish -c Release -o /app/publish

# Этап выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication2.dll"]