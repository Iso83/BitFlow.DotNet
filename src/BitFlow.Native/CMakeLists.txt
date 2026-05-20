cmake_minimum_required(VERSION 3.20)

project(BitFlowNative)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(BF_BUILD_TESTS OFF CACHE BOOL "" FORCE)

# -------------------------------------------------------
# Git hash ophalen van BitFlow submodule
# -------------------------------------------------------

execute_process(
    COMMAND git rev-parse --short HEAD
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/../../extern/BitFlow
    OUTPUT_VARIABLE BITFLOW_GIT_HASH
    OUTPUT_STRIP_TRAILING_WHITESPACE
)

message(STATUS "BitFlow git hash: ${BITFLOW_GIT_HASH}")

configure_file(
    ${CMAKE_CURRENT_SOURCE_DIR}/src/version.h.in
    ${CMAKE_CURRENT_BINARY_DIR}/generated/version.h
    @ONLY
)

# -------------------------------------------------------

add_subdirectory(../../extern/BitFlow extern/BitFlow-build)

add_library(BitFlow.Native SHARED
    include/bitflow_native.h
    src/bitflow_native.cpp
)

target_include_directories(BitFlow.Native PUBLIC
    include
    ${CMAKE_CURRENT_BINARY_DIR}/generated
)

target_link_libraries(BitFlow.Native PRIVATE
    BitFlowCore
    BitFlowIO
)

set_target_properties(BitFlow.Native PROPERTIES
    OUTPUT_NAME "BitFlow.Native"
)