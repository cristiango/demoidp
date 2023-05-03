using System.IO.Compression;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string Clean = "clean";
const string Build = "build";

const string ArtifactsDir = "artifacts";
const string PublishInfra = "publish-pack";

var artifactsPath = Path.Combine(Environment.CurrentDirectory, ArtifactsDir);
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(Clean, () =>
{
    Utils.CleanDirectory(artifactsPath);
    Utils.CleanDirectory($"{tempPath}/app");
});

Target(Build, () =>
{
    Run("dotnet", "build demoidp.sln -c Release");
});

Target(PublishInfra, DependsOn(Clean, Build), () =>
{
    Run("dotnet", $"publish src/backend/demoidp.csproj -r linux-x64 -c Release --sc -p:PublishReadyToRun=false -o {tempPath}/app");
    ZipFile.CreateFromDirectory(Path.Combine(tempPath, "app"),Path.Combine(artifactsPath, "demoidp.zip"));
});

Target("default", DependsOn(PublishInfra));

await RunTargetsAndExitAsync(args);