using Microsoft.WindowsAzure.Storage.Table;

namespace Starship.Azure.Providers.Tables {
    public class AzureTableResult : ITableResult {
        public AzureTableResult(TableResult result) {
        }
    }
}