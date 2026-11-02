FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TodoApp.API/TodoApp.API.csproj TodoApp.API/
COPY TodoApp.Core/TodoApp.Core.csproj TodoApp.Core/
COPY TodoApp.Data/TodoApp.Data.csproj TodoApp.Data/
COPY TodoApp.Services/TodoApp.Services.csproj TodoApp.Services/
RUN dotnet restore TodoApp.API/TodoApp.API.csproj

COPY . .
RUN dotnet publish TodoApp.API/TodoApp.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER app
EXPOSE 8080
ENTRYPOINT [ "dotnet", "TodoApp.API.dll" ]