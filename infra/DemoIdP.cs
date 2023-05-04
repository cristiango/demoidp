using System;
using System.IO;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Kind = Pulumi.AzureNative.Storage.Kind;

namespace infra;

public class DemoIdP
{
    public DemoIdP()
    {
        var config = new Config();
        var location = config.Require("location");
        //Resource group
        var resourceGroup = new ResourceGroup("resource-group", new ResourceGroupArgs
        {
            ManagedBy = "pulumi",
            Location = location
        });

        //AppService plan
        var appServicePlan = new AppServicePlan("servicePlan",
            new AppServicePlanArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Kind = "linux",
                Reserved = true,
                Location = resourceGroup.Location,
                Sku = new SkuDescriptionArgs
                {
                    Name = "S1",
                    Tier = "Standard",
                    Size = "S1",
                    Family = "S",
                    Capacity = 1
                },
            });

        // Create a storage account for the artifacts
        var storageAccount = new StorageAccount("storage",
            new StorageAccountArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Kind = Kind.StorageV2,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS
                }
            });


        // Create a container in the storage account for the artifacts
        var blobContainer = new BlobContainer("container", new BlobContainerArgs
        {
            AccountName = storageAccount.Name,
            ResourceGroupName = resourceGroup.Name,
            PublicAccess = PublicAccess.None
        });


        var blob = new Blob("artifacts", new BlobArgs
        {
            AccountName = storageAccount.Name,
            ContainerName = blobContainer.Name,
            ResourceGroupName = resourceGroup.Name,
            Type = BlobType.Block,
            Source = new FileAsset(Path.Combine(GetRootDirectory(), "artifacts", "demoidp.zip"))
        });

        var codeBlockUrl = SignedBlobReadUrl(blob, blobContainer, storageAccount, resourceGroup);

        var appInsights = new Component("appInsights", new ComponentArgs
        {
            ApplicationType = ApplicationType.Web,
            Kind = "web",
            ResourceGroupName = resourceGroup.Name
        });
        
        var webApp = new WebApp("webapp", new WebAppArgs
        {
            Kind = "app,linux",
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = appServicePlan.Id,
            SiteConfig = new SiteConfigArgs
            {
                AlwaysOn = true,
                LinuxFxVersion = "DOTNETCORE|7.0",
                NetFrameworkVersion = "v7.0",
                AppSettings =
                {
                    new NameValuePairArgs
                    {
                        Name = "WEBSITE_RUN_FROM_PACKAGE",
                        Value = codeBlockUrl
                    },
                    new NameValuePairArgs
                    {
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = appInsights.ConnectionString,
                    },
                },
                HealthCheckPath = "/health",
            },
            HttpsOnly = true,
            Location = resourceGroup.Location,
        });
        //AppService
        //AWS record link

        WebAppUrl = webApp.DefaultHostName.Apply(url => $"https://{url}/");
        ResourceGroup = resourceGroup.Name;
        
    }

    static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account,
        ResourceGroup resourceGroup)
    {
        var serviceSasToken = ListStorageAccountServiceSAS.Invoke(new ListStorageAccountServiceSASInvokeArgs
        {
            AccountName = account.Name,
            Protocols = HttpProtocol.Https,
            SharedAccessStartTime = "2021-01-01",
            SharedAccessExpiryTime = "2030-01-01",
            Resource = SignedResource.C,
            ResourceGroupName = resourceGroup.Name,
            Permissions = Permissions.R,
            CanonicalizedResource = Output.Format($"/blob/{account.Name}/{container.Name}"),
            ContentType = "application/json",
            CacheControl = "max-age=5",
            ContentDisposition = "inline",
            ContentEncoding = "deflate",
        }).Apply(blobSAS => blobSAS.ServiceSasToken);

        return Output.Format(
            $"https://{account.Name}.blob.core.windows.net/{container.Name}/{blob.Name}?{serviceSasToken}");
    }

    [Output("webAppUrl")] public Output<string> WebAppUrl { get; set; }
    [Output("resouceGroup")] public Output<string> ResourceGroup { get; set; }
    static string GetRootDirectory()
    {
        // There are two places where this is executed from
        //      1. At development time from the csproj directory.
        //      2. At deployment time from the infra directory in the deployment container.
        // We need to resolve where the lambda packages are.
        var rootDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent!;
        // var tempDir = rootDirectory.GetDirectories().SingleOrDefault(d => d.Name == "artifacts");
        // if (tempDir != null)
        // {
        //     rootDirectory = tempDir.GetDirectories("app").Single();
        // }
        Log.Info($"Root dir: {rootDirectory.FullName}");
        return rootDirectory.FullName;
    }
}