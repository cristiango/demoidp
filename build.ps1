$tag="demoidp"

docker build `
-f build.dockerfile `
--tag $tag.

docker run --rm --name $tag `
-v /var/run/docker.sock:/var/run/docker.sock `
-v $PWD/artifacts:/repo/artifacts `
--network host `
$tag `
dotnet run --project build/build.csproj -c Release -- $args