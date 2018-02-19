using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Starship.Core.Extensions;
using Starship.Core.Http;

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

        public CloudBlockBlob Upload(string blobName, byte[] data) {
            var blob = Container.GetBlockBlobReference(blobName);
            blob.UploadFromByteArray(data, 0, data.Length);
            return blob;
        }

        public async Task<CloudBlockBlob> UploadAsync(string blobName, byte[] data) {
            var blob = Container.GetBlockBlobReference(blobName);
            await blob.UploadFromByteArrayAsync(data, 0, data.Length);
            return blob;
        }

        public async Task<CloudBlockBlob> UploadAsync(string blobName, string path, string contentType) {
            var blob = Container.GetBlockBlobReference(blobName);
            blob.Properties.ContentType = contentType;

            //await blob.UploadFromFileAsync(path, FileMode.Open);
            await blob.UploadFromFileAsync(path);

            return blob;
        }

        public async Task<CloudBlockBlob> UploadAsync(FileStream stream, string filename) {
            var blob = Container.GetBlockBlobReference(filename);
            await blob.UploadFromStreamAsync(stream);
            return blob;
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