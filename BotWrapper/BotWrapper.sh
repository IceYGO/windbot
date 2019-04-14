#!/bin/bash

cd windbot

if [ "$#" -ne 3 ]; then
    mono WindBot.exe 
else
    command=$1
    flag=$2
    port=$3
    arg="${command//\'/\"}" # replace ' to "
    if [ "$flag" -eq 1 ]; then arg=$arg" Hand=1"; fi
    arg=$arg" Port="$port
    eval "mono WindBot.exe "$arg
fi
