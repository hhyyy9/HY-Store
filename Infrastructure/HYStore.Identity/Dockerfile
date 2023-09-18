FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Infrastructure/HYStore.Identity/HYStore.Identity.csproj", "Infrastructure/HYStore.Identity/"]
RUN dotnet restore "Infrastructure/HYStore.Identity/HYStore.Identity.csproj"
COPY . .
WORKDIR "/src/Infrastructure/HYStore.Identity"
RUN dotnet build "HYStore.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HYStore.Identity.csproj" -c Release -o /app/publish


# Set password for the certificate as 1234
# I'm using Environment Variable here to simplify the .NET Core sample.
ENV certPassword 1234

# Use opnssl to generate a self signed certificate cert.pfx with password $env:certPassword
RUN openssl genrsa -des3 -passout pass:${certPassword} -out server.key 2048
RUN openssl rsa -passin pass:${certPassword} -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=localhost'
RUN openssl x509 -req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out cert.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:${certPassword}

# Expose port 443 for the application.
EXPOSE 443


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HYStore.Identity.dll"]
