cmake_minimum_required(VERSION 3.20)

project(BitFlowNative)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(BF_BUILD_TESTS OFF CACHE BOOL "" FORCE)

add_subdirectory(../../extern/BitFlow extern/BitFlow-build)

add_library(BitFlow.Native SHARED
    include/bitflow_native.h
    src/bitflow_native.cpp
)

target_include_directories(BitFlow.Native PUBLIC
    include
)

target_link_libraries(BitFlow.Native PRIVATE
    BitFlowCore
    BitFlowIO
)

set_target_properties(BitFlow.Native PROPERTIES
    OUTPUT_NAME "BitFlow.Native"
)