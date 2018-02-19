using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Starship.Azure.Providers {
    public class AzureBlobProvider {

        public AzureBlobProvider(string connectionstring) {
            Account = CloudStorageAccount.Parse(connectionstring);
        }

        public AzureBlobProvider(CloudStorageAccount account) {
            Account = account;
        }

        public void Upload(Byte[] data, string containerName, string filename) {
            GetContainer(containerName).Upload(filename, data);
        }

        public async Task UploadAsync(FileStream stream, string containerName, string filename) {
            await GetContainer(containerName).UploadAsync(stream, filename);
        }

        public AzureBlobStorageContainer GetContainer(string containerName) {
            if (!Containers.ContainsKey(containerName)) {
                Containers.Add(containerName, new AzureBlobStorageContainer(GetClient().GetContainerReference(containerName)));
                Containers[containerName].Create();
            }

            return Containers[containerName];
        }

        private CloudBlobClient GetClient() {
            return Account.CreateCloudBlobClient();
        }

        private CloudStorageAccount Account { get; set; }

        private static Dictionary<string, AzureBlobStorageContainer> Containers = new Dictionary<string, AzureBlobStorageContainer>();
    }
}