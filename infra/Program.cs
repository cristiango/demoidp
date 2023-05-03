using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Pulumi;
using Pulumi.Command.Local;
using AzureNative = Pulumi.AzureNative;
using SyncedFolder = Pulumi.SyncedFolder;

return await Deployment.RunAsync(() =>
{
    var demoIdP = new infra.DemoIdP();
    
    return new Dictionary<string, object?>
        {
            ["siteURL"] = demoIdP.WebAppUrl,
            ["resourceGroup"] = demoIdP.ResourceGroup,
        };
});
