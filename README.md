# demoidp
This project is meant to be a learning exercise on how to deploy a .NET web app in Azure using Pulumi

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

1. Docker
1. Pulumi
1. Azure CLI

### Build With
The project is built in docker to isolate from the local environment.
As a result of the build will be a zip file in `artifacts` dir
```powershell
./build.ps1
```

### Deployment
TO Improve - add deployment project
```powershell
# navigate to infra folder
cd ./infra
pulumi up
```

## Acknowledgments
Code is using [Duende identityserver](https://duendesoftware.com/products/identityserver)