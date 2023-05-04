using System.IO.Compression;
using Bullseye.Internal;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string Clean = "clean";
const string Build = "build";

const string ArtifactsDir = "artifacts";
const string PublishFrontEnd = "publish-frontend";
const string PublishBackEnd = "publish-backend";
const string Publish = "publish-pack";

var artifactsPath = Path.Combine(Environment.CurrentDirectory, ArtifactsDir);
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(Clean, () =>
{
    Utils.CleanDirectory(artifactsPath);
    Utils.CleanDirectory($"{tempPath}/app");
});

Target(Build, () => { Run("dotnet", "build demoidp.sln -c Release"); });

Target(PublishFrontEnd, DependsOn(Clean), () =>
{
    const string sourceDir = "src/backend/wwwroot";
    var destDir = Path.Combine(artifactsPath, "frontend");
    Directory.CreateDirectory(destDir);
    foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
    foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        File.Copy(filePath, filePath.Replace(sourceDir, destDir), true);
});

Target(PublishBackEnd, DependsOn(Clean, Build), () =>
{
    Run("dotnet",
        $"publish src/backend/demoidp.csproj -r linux-x64 -c Release --sc -p:PublishReadyToRun=false -o {tempPath}/app");
    ZipFile.CreateFromDirectory(Path.Combine(tempPath, "app"), Path.Combine(artifactsPath, "demoidp.zip"));
});

Target(Publish, DependsOn(PublishFrontEnd, PublishBackEnd));

Target("default", DependsOn(Publish));

await RunTargetsAndExitAsync(args);