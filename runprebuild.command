#!/bin/sh
cd `dirname $0`
mono bin/Prebuild.exe /target vs2008 /targetframework v3_5
if [ -d ".git" ]; then git log --pretty=format:"Virtual Reality Software: 0.1.1.2 Dev (%cd.%h)" --date=short -n 1 > bin/.version; fi
