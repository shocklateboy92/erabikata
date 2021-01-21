#!/bin/bash

if [[ -e "./init.sh" ]]; then

    export PATH="$(pwd)/ingestion:${PATH}";
    [[ -e ./anime-subs ]] || ln -sf "$IKNOW_DIR/anime-subs" ./anime-subs

else
    echo "Error: Must be sourced in $ERAB_DIR";
fi

