#!/bin/sh
mono bin/Prebuild.exe /target vs2010 /targetframework v4_0 /conditionals NET_4_0
if [ -d ".git" ]; then git log --pretty=format:"Virtual Reality:0.1.1.19 Dev:0.5.1 Rev:%h" -n 1 > bin/.version; fi
