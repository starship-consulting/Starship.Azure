using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Starship.Core.Storage;

namespace Starship.Azure.Providers.Storage {
    public class AzureBlobStorageProvider : IsFileStorageProvider {

        public AzureBlobStorageProvider(string connectionstring) {
            Account = CloudStorageAccount.Parse(connectionstring);
        }

        public AzureBlobStorageProvider(CloudStorageAccount account) {
            Account = account;
        }

        static AzureBlobStorageProvider() {
            Containers = new Dictionary<string, CloudBlobContainer>();
        }

        public async Task<FileReference> UploadAsync(string container, Stream stream, string path) {
            var blob = GetContainer(container).GetBlockBlobReference(path);
            await blob.UploadFromStreamAsync(stream);
            return new AzureBlobFileReference(blob);
        }

        public async Task<FileReference> UploadAsync(string container, byte[] data, string path) {
            var blob = GetContainer(container).GetBlockBlobReference(path);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return new AzureBlobFileReference(blob);
        }

        public async Task<IEnumerable<FileReference>> ListFilesAsync(string container, string path) {
            BlobContinuationToken continuationToken = null;
            var results = new List<ICloudBlob>();

            do {
                var files = await GetContainer(container).ListBlobsSegmentedAsync(path, true, BlobListingDetails.Metadata, 100, continuationToken, null, null);
                results.AddRange(files.Results.Cast<ICloudBlob>());
                continuationToken = files.ContinuationToken;
            } while (continuationToken != null);

            return results.Select(blob => new AzureBlobFileReference(blob));
        }

        public async Task<bool> DeleteAsync(string container, string path) {
            var blob = GetContainer(container).GetBlockBlobReference(path);
            await blob.DeleteAsync();
            return true;
        }

        public async Task<FileReference> GetFileAsync(string container, string path) {
            var reference = GetContainer(container).GetBlockBlobReference(path);
            var stream = await reference.OpenReadAsync();
            return new AzureBlobFileReference(reference, stream);
        }

        private CloudBlobContainer GetContainer(string containerName) {
            if (!Containers.ContainsKey(containerName)) {
                Containers.Add(containerName, GetClient().GetContainerReference(containerName));
                Containers[containerName].CreateIfNotExistsAsync().Wait();
            }

            return Containers[containerName];
        }

        private CloudBlobClient GetClient() {
            return Account.CreateCloudBlobClient();
        }

        private CloudStorageAccount Account { get; set; }

        private static readonly Dictionary<string, CloudBlobContainer> Containers;
    }
}