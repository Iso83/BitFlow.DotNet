FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

RUN apt-get update && apt-get install -y \
    cmake \
    build-essential

WORKDIR /src

COPY . .

RUN cmake -S src/BitFlow.Native -B src/BitFlow.Native/build

# RUN cmake --build src/BitFlow.Native/build --config Release
RUN cmake --build src/BitFlow.Native/build --config Release --verbose

RUN dotnet publish src/BitFlow.Web/BitFlow.Web.csproj \
    -c Release \
    -o /app/publish