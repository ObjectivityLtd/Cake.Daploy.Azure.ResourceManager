namespace Cake.Deploy.Azure
{
    using Microsoft.Azure.Management.ResourceManager;
    using Microsoft.Azure.Management.ResourceManager.Models;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class AzureTemplateDeployment
    {
        private string name;
        private ServiceClientCredentials credentials;
        private string subscriptionId;
        private string resourceGroupName;
        private object template;

        private readonly DeploymentParameters parameters = new DeploymentParameters();

        internal AzureTemplateDeployment(string name)
        {
            this.name = name;
        }

        public AzureTemplateDeployment SetCredentials(ServiceClientCredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            this.credentials = credentials;

            return this;
        }

        public AzureTemplateDeployment SetSubscription(string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            this.subscriptionId = subscriptionId;

            return this;
        }

        public AzureTemplateDeployment SetResourceGroupName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.resourceGroupName = name;

            return this;
        }

        public AzureTemplateDeployment SetTemplateFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new ArgumentException($"File does not exist: {fileInfo.FullName}");
            }

            this.template = JsonConvert.DeserializeObject(File.ReadAllText(fileInfo.FullName));

            return this;
        }

        public AzureTemplateDeployment AddDeploymentParameter(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.parameters.AddParameter(name, value);

            return this;
        }

        public void Run()
        {
            var rmc = new ResourceManagementClient(this.credentials);
            rmc.SubscriptionId = this.subscriptionId;

            JObject parametersObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this.parameters)) as JObject;

            JObject parameters = parametersObject["Parameters"].ToObject<JObject>();

            var deployment = new Deployment()
            {
                Properties = new DeploymentProperties()
                {
                    Mode = DeploymentMode.Incremental,
                    Template = this.template,
                    Parameters = parameters
                }
            };

            var deploymentResult = rmc.Deployments.CreateOrUpdate(this.resourceGroupName, this.name, deployment);
        }
    }

    public class DeploymentParameters
    {

        private readonly Dictionary<string, ParameterDefinition> parameters = new Dictionary<string, ParameterDefinition>();

        public Dictionary<string, ParameterDefinition> Parameters => this.parameters;

        public void AddParameter(string name, object value)
        {
            if (this.parameters.ContainsKey(name))
            {
                throw new ArgumentException($"Key already exists: {name}");
            }

            var param = new ParameterDefinition(value);

            this.parameters.Add(name, param);
        }

        public class ParameterDefinition
        {
            object value;

            public object Value => this.value;

            internal ParameterDefinition(object value)
            {
                this.value = value;
            }
        }
    }
}
