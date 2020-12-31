#!/bin/bash

set -xe

pushd ./srt-parser
dotnet build
popd

pushd analyzer
mvn package
if [[ ! -e dict/system_full.dic ]]; then
    wget 'http://sudachi.s3-website-ap-northeast-1.amazonaws.com/sudachidict/sudachi-dictionary-20201223-full.zip' -O dict.zip
    unzip -jd dict dict.zip
fi
popd

pushd python
pipenv install
popd
