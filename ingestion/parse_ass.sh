#!/bin/bash

set -e

root="$(pwd)"

cd "$(dirname $BASH_SOURCE)/ass-parser"

pipenv run python ./ass-extractor.py "$root"
