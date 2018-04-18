using System;
using ConsumeFtpFiles.Clients;
using ConsumeFtpFiles.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Functions
{
    public static class LogWorker
    {
        [FunctionName("LogWorker")]
        public static void Run([QueueTrigger("filelogs")]string myQueueItem, TraceWriter log)
        {
            try
            {
                BlobClient blobClient = new BlobClient(CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
                var item = JsonConvert.DeserializeObject<FileEntry>(myQueueItem);
                blobClient.writeLogFile(new FileEntryEntity(item.PartitionKey, item.RowKey)
                {
                    dateTime_VC = item.dateTime_VC,
                    filename_VC = item.filename_VC,
                    filetype_VC = item.filetype_VC,
                    fileurl_VC = item.fileurl_VC
                });
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.Message}");
                throw ex;
            }
        }
    }
}
