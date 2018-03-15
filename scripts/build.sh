#!/bin/bash
MYGET_ENV=""
case "$TRAVIS_BRANCH" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

#Here be dragons
sed -i -e "s/dnc-dshop/dnc-dshop$MYGET_ENV/g" MyGet.config
mv MyGet.config Nuget.config

dotnet build -c Release