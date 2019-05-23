FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["OOOVote/OOOVote.csproj", "OOOVote/"]
RUN dotnet restore "OOOVote/OOOVote.csproj"
COPY . .
WORKDIR "/src/OOOVote"
RUN dotnet build "OOOVote.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OOOVote.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OOOVote.dll"]