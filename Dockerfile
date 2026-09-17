FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY src/HotelChecklist.Domain/HotelChecklist.Domain.csproj src/HotelChecklist.Domain/
COPY src/HotelChecklist.Api/HotelChecklist.Api.csproj src/HotelChecklist.Api/
RUN dotnet restore src/HotelChecklist.Api/HotelChecklist.Api.csproj

COPY src/ src/
RUN dotnet publish src/HotelChecklist.Api/HotelChecklist.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/uploads /app/logs \
    && chown -R app:app /app

COPY --from=build --chown=app:app /app .

USER app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HotelChecklist.Api.dll"]
