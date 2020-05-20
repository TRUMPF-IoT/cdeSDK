# SPDX-FileCopyrightText: 2013-2020 TRUMPF Laser GmbH, authors: C-Labs

# SPDX-License-Identifier: MPL-2.0

FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["MyTestHost2/MyTestHost2.csproj", "MyTestHost2/"]
COPY ["../CDMyChatSample/CDMyChat.csproj", "../CDMyChatSample/"]
RUN dotnet restore "MyTestHost2/MyTestHost2.csproj"
COPY . .
WORKDIR "/src/MyTestHost2"
RUN dotnet build "MyTestHost2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTestHost2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTestHost2.dll"]
