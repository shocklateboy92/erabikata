#!/bin/bash

set -xe

cd "$(dirname $BASH_SOURCE)"

pushd ./srt-parser
dotnet build
popd

pushd python
pipenv install
popd
