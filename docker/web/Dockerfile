FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

RUN apt-get update && apt-get install -y \
    cmake \
    build-essential

ARG BITFLOW_GIT_HASH=unknown
ENV BITFLOW_GIT_HASH=${BITFLOW_GIT_HASH}

WORKDIR /src

COPY . .

RUN cmake -S src/BitFlow.Native -B src/BitFlow.Native/build

RUN cmake --build src/BitFlow.Native/build --config Release
# RUN cmake --build src/BitFlow.Native/build --config Release --verbose

RUN dotnet publish src/BitFlow.Web/BitFlow.Web.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

COPY --from=build /app/publish .

COPY --from=build /src/src/BitFlow.Native/build/libBitFlow.Native.so .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BitFlow.Web.dll"]