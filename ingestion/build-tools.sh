#!/bin/bash

set -xe

cd "$(dirname $BASH_SOURCE)"

pushd python
pipenv install
popd
