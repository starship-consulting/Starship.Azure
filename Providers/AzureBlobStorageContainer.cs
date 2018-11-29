using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Starship.Azure.Providers.Storage;
using Starship.Core.Data.Storage;
using Starship.Core.Extensions;
using Starship.Core.Http;
using Starship.Core.Storage;

namespace Starship.Azure.Providers {
    public class AzureBlobStorageContainer {
        public AzureBlobStorageContainer(CloudBlobContainer container) {
            Container = container;
        }

        public AzureBlobStorageContainer Create() {
            Container.CreateIfNotExists();
            return this;
        }

        public async Task<AzureBlobStorageContainer> CreateAsync() {
            await Container.CreateIfNotExistsAsync();
            return this;
        }

        public IEnumerable<IListBlobItem> ListBlobs() {
            return Container.ListBlobs();
        }
        
        public async Task<IEnumerable<FileReference>> ListFilesAsync(string path) {
            BlobContinuationToken continuationToken = null;
            var results = new List<ICloudBlob>();

            do {
                var files = await Container.ListBlobsSegmentedAsync(path, true, BlobListingDetails.Metadata, 100, continuationToken, null, null);
                results.AddRange(files.Results.Cast<ICloudBlob>());
                continuationToken = files.ContinuationToken;
            } while (continuationToken != null);

            return results.Select(blob => new AzureBlobFileReference(blob));
        }

        public bool Delete(string blobName) {
            var blob = Container.GetBlockBlobReference(blobName);
            blob.Delete();
            return true;
        }

        public async Task<bool> DeleteAsync(string blobName) {
            var blob = Container.GetBlockBlobReference(blobName);
            await blob.DeleteAsync();
            return true;
        }

        public UploadedFile Upload(string blobName, byte[] data) {
            var blob = Container.GetBlockBlobReference(blobName);
            blob.UploadFromByteArray(data, 0, data.Length);
            return new UploadedFile();
        }

        public async Task<FileReference> UploadAsync(string blobName, byte[] data) {
            var blob = Container.GetBlockBlobReference(blobName);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return new AzureBlobFileReference(blob);
        }

        public async Task<CloudBlockBlob> UploadAsync(string blobName, string path, string contentType) {
            var blob = Container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = contentType;

            //await blob.UploadFromFileAsync(path, FileMode.Open);
            await blob.UploadFromFileAsync(path);

            return blob;
        }

        public async Task<FileReference> UploadAsync(Stream stream, string filename) {
            var blob = Container.GetBlockBlobReference(filename);
            await blob.UploadFromStreamAsync(stream);
            return new AzureBlobFileReference(blob);
        }

        public async Task<string> ReadAsync(string filename) {
            return await Container.GetBlockBlobReference(filename).DownloadTextAsync();
        }

        public string Read(string filename) {
            return Container.GetBlockBlobReference(filename).DownloadText();
        }

        public FileDetails Download(string filename, string path = "") {
            var blob = Container.GetBlockBlobReference(filename);

            if (path.IsEmpty()) {
                path = Path.GetTempFileName();
            }

            blob.DownloadToFile(path, FileMode.Create);

            return new FileDetails {
                Name = filename,
                Size = blob.Properties.Length,
                ContentType = blob.Properties.ContentType,
                Location = path
            };
        }

        public async Task<FileReference> GetFileAsync(string path) {
            var reference = Container.GetBlockBlobReference(path);
            var stream = await reference.OpenReadAsync();
            return new AzureBlobFileReference(reference, stream);
        }
        
        public async Task<FileDetails> DownloadAsync(string filename) {
            var blob = Container.GetBlockBlobReference(filename);
            var path = Path.GetTempFileName();
            await blob.DownloadToFileAsync(path, FileMode.Create);

            return new FileDetails {
                Name = filename,
                Size = blob.Properties.Length,
                ContentType = blob.Properties.ContentType,
                Location = path
            };
        }

        public async Task Clear() {
            foreach (CloudBlockBlob blob in ListBlobs()) {
                await blob.DeleteIfExistsAsync();
            }
        }

        private CloudBlobContainer Container { get; set; }
    }
}