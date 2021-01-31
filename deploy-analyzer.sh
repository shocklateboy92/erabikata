#!/bin/bash

set -xe

analyzer_dir=ingestion/server
image_name=registry.apps.lasath.org/erabikata_analyzer

docker build -t $image_name $analyzer_dir
docker push $image_name

ssh mccoy /home/data_user/mccoy-vnext/update-erabikata.sh

