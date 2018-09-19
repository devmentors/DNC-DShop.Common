# Distributed .NET Core

![DevMentors](https://github.com/devmentors/DNC-DShop/blob/master/assets/devmentors_logo.png)

**What is Distributed .NET Core?**
----------------

It's an open source project (and a course available soon at [devmentors.io](https://devmentors.io)), providing in-depth knowledge about building microservices using [.NET Core](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial) framework and variety of tools. One of the goals, was to create a cloud agnostic solution, that you shall be able to run anywhere. 

We encourage you to join our [Discourse](https://www.discourse.org) forum available at [forum.devmentors.io](https://forum.devmentors.io).

For this particular course, please have a look at the topics being discussed under this [category](https://forum.devmentors.io/c/courses/distributed-dotnet-core).

**What is DShop.Common?**
----------------

DShop.Common is a shared library, containing infrastructural code and cross-cutting concerns for [DShop](https://github.com/devmentors/DNC-DShop) solution.

|Branch             |Build status                                                  
|-------------------|-----------------------------------------------------
|master             |[![master branch build status](https://api.travis-ci.org/devmentors/DNC-DShop.Common.svg?branch=master)](https://travis-ci.org/devmentors/DNC-DShop.Common)
|develop            |[![develop branch build status](https://api.travis-ci.org/devmentors/DNC-DShop.Common.svg?branch=develop)](https://travis-ci.org/devmentors/DNC-DShop.Common/branches)


**How to install the package?**
----------------

For now, `DShop.Common` is available under a custom [MyGet](https://myget.org) feed, thus, whenever you execute e.g. `dotnet restore/build/run/publish/test` command, provide the `--source https://api.nuget.org/v3/index.json --source https://www.myget.org/F/dnc-dshop/api/v3/index.json` arguments for `-c Release` builds.

You can also clone the repository locally into the same root directory amongst other `DShop` repositories, and it will be restored from your disk while building in the default `Debug` mode.