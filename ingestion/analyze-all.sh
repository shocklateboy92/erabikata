
#!/bin/bash

set -e

export BASE_PATH="$(dirname $BASH_SOURCE)/analyzer";
java -jar "$BASE_PATH/target/analyzer-1.0-SNAPSHOT-jar-with-dependencies.jar"
