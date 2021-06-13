#!/bin/bash

set -xe

image_name=registry.apps.lasath.org/erabikata3
backend_dir=backend/Erabikata.Backend
backend_build_dir="$backend_dir/bin/Release"
www_dir="$backend_build_dir/net5.0/publish/wwwroot"
frontend_build_dir=frontend/build

# Clear old crap, in case I haven't done a git clean
rm -rf $backend_build_dir
rm -rf $frontend_build_dir

dotnet publish --configuration=Release $backend_dir/*.csproj

MAKEFLAGS="-j13" yarn --cwd frontend install
MAKEFLAGS="-j13" yarn --cwd frontend rtk
yarn --cwd frontend build
cp -rvf "$frontend_build_dir" "$www_dir"

docker build -t $image_name $backend_dir
docker push $image_name

ssh mccoy.home.lasath.org /home/data_user/mccoy-vnext/update-erabikata.sh
