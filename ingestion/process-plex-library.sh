#!/bin/bash

set -e

root="$(pwd)";
cd "$(dirname $BASH_SOURCE)/python";

pipenv run python ./plex-processor.py "$root" "$@";
