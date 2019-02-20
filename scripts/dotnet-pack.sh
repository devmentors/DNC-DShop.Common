#!/bin/bash
echo Executing after success scripts on branch $TRAVIS_BRANCH
echo Triggering MyGet package build

cd src/DShop.Common
dotnet pack /p:PackageVersion=2.0.$TRAVIS_BUILD_NUMBER --no-restore -o .

echo Uploading DShop.Common package to MyGet using branch $TRAVIS_BRANCH

case "$TRAVIS_BRANCH" in
  "master")
    dotnet nuget push *.nupkg -k $MYGET_API_KEY -s https://www.myget.org/F/dnc-dshop/api/v2/package
    ;;
  "develop")
    dotnet nuget push *.nupkg -k $MYGET_DEV_API_KEY -s https://www.myget.org/F/dnc-dshop-dev/api/v2/package
    ;;    
esac