#!/bin/bash

set -e

echo "==== BitFlow Update Start ===="

cd ~/dev/github/BitFlow.DotNet

echo ""
echo "==== Git Pull ===="
git pull --recurse-submodules

echo ""
echo "==== Submodules Update ===="
git submodule update --init --recursive

echo ""
echo "==== Docker Compose Down ===="
docker compose down --remove-orphans

echo ""
echo "==== Docker Builder Cleanup ===="
docker builder prune -af

export BITFLOW_GIT_HASH=$(git -C extern/BitFlow rev-parse --short HEAD)
echo ""
echo "==== BitFlow Git Hash ===="
echo "$BITFLOW_GIT_HASH"

echo ""
echo "==== Docker Compose Build ===="
docker compose build --no-cache