#!/bin/bash

set -xe

image_name=registry.apps.lasath.org/erabikata3
backend_dir=backend/Erabikata.Backend
backend_build_dir="$backend_dir/bin/Release"
www_dir="$backend_build_dir/net6.0/publish/wwwroot"
frontend_build_dir=frontend/build
configuration=--configuration=Release

# Clear old crap, in case I haven't done a git clean
rm -rf $backend_build_dir
rm -rf $frontend_build_dir

dotnet publish $configuration $backend_dir/*.csproj

DOTNET_ARGS="$configuration" MAKEFLAGS="-j13" yarn --cwd frontend install
yarn --cwd frontend build
cp -rvf "$frontend_build_dir" "$www_dir"

docker build -t $image_name $backend_dir
docker push $image_name

ssh mccoy.home.lasath.org /home/data_user/mccoy-vnext/update-erabikata.sh
