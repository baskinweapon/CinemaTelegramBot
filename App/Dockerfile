# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копируем CSPROJ и восстанавливаем зависимости
COPY App.csproj ./
RUN dotnet build

# Копируем остальные файлы и собираем приложение
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Устанавливаем точку входа
ENTRYPOINT ["dotnet", "out.dll"]
