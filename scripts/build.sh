#!/bin/bash
MYGET_ENV=""
case "$TRAVIS_BRANCH" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

sed -i -e "s/dnc-dshop/dnc-dshop$MYGET_ENV/g" Nuget-release.config

mv Nuget-release.config Nuget.config
dotnet build -c Release