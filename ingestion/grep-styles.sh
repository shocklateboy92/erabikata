#!/bin/bash

grep Style: *.ass -h | cut -d, -f1 | sort -u 

