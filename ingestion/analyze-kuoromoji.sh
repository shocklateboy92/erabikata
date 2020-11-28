
#!/bin/bash

set -e

java -jar "$(dirname $BASH_SOURCE)/analyzer/target/analyzer-1.0-SNAPSHOT-jar-with-dependencies.jar"
