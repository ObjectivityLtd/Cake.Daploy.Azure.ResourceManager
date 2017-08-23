# Cake.Deploy.Azure.ResourceManager ![Build status](https://ci.appveyor.com/api/projects/status/github/ObjectivityLtd/Cake.Deploy.Azure.ResourceManager?svg=true)
Cake addin for deploying Azure resources through the Azure Resource Manager using templates.

# How to add Cake.Deploy.Azure.ResourceManager
In order to use it add the following line in your addin section:
```cake
#addin "Cake.Deploy.Azure.ResourceManager"
```

# How to use Cake.Deploy.Azure.ResourceManager
ServiceClientCredentials object required by Cake.Deploy.Azure.ResourceManager can be obtained using Cake.Deploy.Azure.Authentication addin.

Example of deploying resources to newly created group, using saved locally template from https://github.com/Azure/azure-quickstart-templates/tree/master/201-web-app-sql-database :

```cake
#addin "Cake.Deploy.Azure.Authentication"
#addin "Cake.Deploy.Azure.ResourceManager"

Task("ProvisionResources")
    .Does(() =>
{
    var resourceGroupName = "exampleGroup";

    var credentials = LoginAzureRM("tenantId", "login", "password");
    var deployment = AzureTemplateDeployment("deploymentName");
    deployment.SetCredentials(credentials);
    deployment.SetSubscription("subscriptionId");

    deployment.CreateResourceGroup(resourceGroupName, "West Europe");

    deployment.SetResourceGroupName(resourceGroupName)
        .SetTemplateFromFile(".\\templates\\azuredeploy.json")
        .AddDeploymentParameter("sqlAdministratorLogin", "dbLogin")
        .AddDeploymentParameter("sqlAdministratorLoginPassword", "dbPassword")
        .Run();
});

RunTarget("ProvisionResources");
```

# AzureTemplateDeployment API
Cake.Deploy.Azure.ResourceManager exposes single method creating AzureTemplateDeployment object, with public members:
```csharp
AzureTemplateDeployment SetCredentials(ServiceClientCredentials credentials)
AzureTemplateDeployment SetSubscription(string subscriptionId)
AzureTemplateDeployment SetResourceGroupName(string name)
AzureTemplateDeployment SetTemplateFromFile(string filePath)
AzureTemplateDeployment AddDeploymentParameter(string name, object value)
void Run()

void CreateResourceGroup(string name, string location)
```