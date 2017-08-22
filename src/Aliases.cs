namespace Cake.Daploy.Azure.ResourceManager
{
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Deploy.Azure;

    [CakeAliasCategory("Sample")]
    public static class AddinAliases
    {
        [CakeMethodAlias]
        public static AzureTemplateDeployment AzureTemplateDeployment(this ICakeContext ctx, string name)
        {
            return new AzureTemplateDeployment(name);
        }
    }
}
