using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Starship.Azure.Providers.Tables;
using Starship.Core.Storage;

namespace Starship.Azure.Providers.Storage {
    public class AzureVirtualFileStorageProvider : IsFileStorageProvider {

        public AzureVirtualFileStorageProvider(string connectionstring) {
            BlobProvider = new AzureBlobStorageProvider(connectionstring);
            TableProvider = new AzureTableStorageProvider(connectionstring);
        }

        public async Task<FileReference> UploadAsync(string partition, Stream stream, string path) {
            var file = new StoredFileReference(partition, path);
            var reference = await BlobProvider.UploadAsync(partition, stream, file.RowKey);
            return await SaveFileReference(file, reference);
        }

        public async Task<FileReference> UploadAsync(string partition, byte[] data, string path) {
            var file = new StoredFileReference(partition, path);
            var reference = await BlobProvider.UploadAsync(partition, data, file.RowKey);
            return await SaveFileReference(file, reference);
        }

        private async Task<FileReference> SaveFileReference(StoredFileReference file, FileReference reference) {
            file.WriteTo(reference);
            TableProvider.Add(file);
            await TableProvider.SaveAsync();
            return reference;
        }

        public async Task<IEnumerable<FileReference>> ListFilesAsync(string partition, string path) {
            var files = TableProvider.Get<StoredFileReference>().ToList();

            return await BlobProvider.ListFilesAsync(partition, path);
        }

        public async Task<bool> DeleteAsync(string partition, string path) {
            return await BlobProvider.DeleteAsync(partition, path);
        }

        public async Task<FileReference> GetFileAsync(string partition, string path) {
            return await BlobProvider.GetFileAsync(partition, path);
        }

        private AzureTableStorageProvider TableProvider { get; set; }

        private AzureBlobStorageProvider BlobProvider { get; set; }
    }
}