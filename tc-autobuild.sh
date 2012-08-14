#!/bin/bash

mono bin/Prebuild.exe /target vs2008 /targetframework v3_5

if [ -d ".git" ]; then git log --pretty=format:"Virtual Reality:0.1.1.19 Dev:0.5.1 Rev:%h" -n 1 > bin/.version; fi

unset makebuild
unset makedist

while [ "$1" != "" ]; do
    case $1 in
	build )       makebuild=yes
                      ;;
	dist )        makedist=yes
                      ;;
    esac
    shift
done

if [ "$makebuild" = "yes" ]; then
    xbuild VirtualReality.sln
    res=$?

    if [ "$res" != "0" ]; then
	exit $res
    fi

    if [ "$makedist" = "yes" ]; then
	rm -f virtualreality-autobuild.tar.bz2
	tar cjf virtualreality-autobuild.tar.bz2 bin
    fi
fi
