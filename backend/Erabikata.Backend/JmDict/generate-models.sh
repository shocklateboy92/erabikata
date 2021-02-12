#!/bin/bash

# Create manfist / install
# dotnet tool install dotnet-xscgen
# dotnet new tool-manifest

dotnet tool run xscgen -- -n =Erabikata.Backend.Models.Input.Generated --nullable ./jmdict_e.xsd -v

