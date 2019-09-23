using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Starship.Azure.Providers {
    public class AzureQueueProvider {

        public AzureQueueProvider(string connectionstring) {
            Client = CloudStorageAccount.Parse(connectionstring).CreateCloudQueueClient();
        }

        public void Enqueue(string queueName, object message) {
            var queue = GetQueue(queueName);
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            queue.AddMessage(queueMessage);
        }

        public async Task EnqueueAsync(string queueName, object message) {
            var queue = await GetQueueAsync(queueName);
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            await queue.AddMessageAsync(queueMessage);
        }

        public async Task<CloudQueue> GetQueueAsync(string queueName) {

            await Semaphore.WaitAsync();

            try {
                if(!Queues.ContainsKey(queueName)) {
                    var queue = Client.GetQueueReference(queueName);
                    await queue.CreateIfNotExistsAsync();
                }

                return Queues[queueName];
            }
            finally {
                Semaphore.Release();
            }
        }

        public CloudQueue GetQueue(string queueName) {
            
            try {
                if(!Queues.ContainsKey(queueName)) {
                    var queue = Client.GetQueueReference(queueName);
                    queue.CreateIfNotExists();
                }

                return Queues[queueName];
            }
            finally {
                Semaphore.Release();
            }
        }

        private CloudQueueClient Client { get; set; }

        private static readonly Dictionary<string, CloudQueue> Queues = new Dictionary<string, CloudQueue>();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
    }
}