using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cake.Deploy.Azure.ResourceManager
{
    public static class StorageOperations
    {
        public static void CreateBlobContainer(string storageAccountName, string storageAccountKey, string containerName)
        {
            if (containerName.Any(char.IsUpper))
            {
                throw new ArgumentException("containerName must be lower case");
            }

            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                                             + "AccountName=" + storageAccountName
                                             + ";AccountKey=" + storageAccountKey
                                             + ";EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();
        }
    }
}
