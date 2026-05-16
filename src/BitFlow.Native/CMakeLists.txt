cmake_minimum_required(VERSION 3.20)

project(BitFlowNative)

add_subdirectory(../../extern/BitFlow extern/BitFlow-build)

add_library(BitFlow.Native SHARED
    src/bitflow_native.cpp
)

target_include_directories(BitFlow.Native PUBLIC
    include
)