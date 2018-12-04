using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using Starship.Core.Storage;

namespace Starship.Azure.Providers.Storage {
    public class AzureBlobFileReference : FileReference {

        public AzureBlobFileReference() {
        }

        public AzureBlobFileReference(ICloudBlob blob) {

            var segments = blob.Uri.AbsolutePath.Split('/');
            DateTime lastModified;

            if(blob.Metadata.ContainsKey("LastModified")) {
                lastModified = DateTime.ParseExact(blob.Metadata["LastModified"], "o", null, DateTimeStyles.AdjustToUniversal);
            }
            else {
                lastModified = blob.Properties.LastModified.Value.UtcDateTime;
            }

            Path = string.Join("/", segments.Take(segments.Length - 1));
            //Name = segments.Last();
            //Url = blob.Uri.ToString();
            LastModified = lastModified;
            Blob = blob;
        }

        public AzureBlobFileReference(ICloudBlob blob, Stream stream) : this(blob) {
            Length = blob.Properties.Length;
            ContentType = blob.Properties.ContentType;
            Stream = stream;
        }
        
        private ICloudBlob Blob { get; set; }
    }
}