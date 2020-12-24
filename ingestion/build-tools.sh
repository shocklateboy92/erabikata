#!/bin/bash

set -xe

pushd ./srt-parser
dotnet build
popd

pushd analyzer
mvn package
popd

pushd python
pipenv install
popd
