# SPDX-FileCopyrightText: 2013-2020 TRUMPF Laser GmbH, authors: C-Labs
#
# SPDX-License-Identifier: MPL-2.0

FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["MyTestHost/MyTestHost.csproj", "MyTestHost/"]
COPY ["../CDMyHistorianSample/CDMyHistorianSample.csproj", "../CDMyHistorianSample/"]
COPY ["../CDMyChatSample/CDMyChat.csproj", "../CDMyChatSample/"]
RUN dotnet restore "MyTestHost/MyTestHost.csproj"
COPY . .
WORKDIR "/src/MyTestHost"
RUN dotnet build "MyTestHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTestHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTestHost.dll"]
