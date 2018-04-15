using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumeFtpFiles.Clients;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace Functions
{
    public static class GetFilesFunction
    {
        [FunctionName("GetFilesFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger function processed a request.");
                BlobClient blobClient = new BlobClient(CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
                var fileList = blobClient.getFiles();
                return fileList == null || fileList.Count() == 0
                    ? req.CreateResponse(HttpStatusCode.NotFound, "No results found")
                    : req.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, $"Guru Meditation #{ex.Message}");
            }
        }
    }
}
