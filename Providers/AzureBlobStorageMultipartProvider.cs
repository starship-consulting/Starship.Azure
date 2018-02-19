using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Starship.Core.Http;

namespace Starship.Azure.Providers {
    public class AzureBlobStorageMultipartProvider : MultipartFileStreamProvider {

        public AzureBlobStorageMultipartProvider(AzureBlobStorageContainer container) : base(Path.GetTempPath()) {
            Container = container;
            Files = new List<FileDetails>();
        }

        public override Task ExecutePostProcessingAsync() {

            if (!FileData.Any()) {
                throw new Exception("No files uploaded.");
            }

            if (!AllowMultipleFiles && FileData.Count > 1) {
                throw new Exception("You must upload a single file at a time.");
            }

            var file = FileData.First();
            var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));

            return Container.UploadAsync(fileName, file.LocalFileName, file.Headers.ContentType.MediaType).ContinueWith(upload => {

                var blob = upload.Result;

                File.Delete(file.LocalFileName);

                Files.Add(new FileDetails {
                    ContentType = blob.Properties.ContentType,
                    Name = blob.Name,
                    Size = blob.Properties.Length,
                    Location = blob.Uri.AbsoluteUri
                });

                return base.ExecutePostProcessingAsync();
            });
        }

        public bool AllowMultipleFiles { get; set; }

        public List<FileDetails> Files { get; set; }

        private AzureBlobStorageContainer Container { get; set; }
    }
}