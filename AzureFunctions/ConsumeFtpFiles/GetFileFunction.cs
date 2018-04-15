using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ConsumeFtpFiles.Clients;
using ConsumeFtpFiles.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace Functions
{
    public static class GetFileFunction
    {
        [FunctionName("GetFileFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("C# HTTP trigger function processed a request.");
                var file = new FileEntryEntity();
                string rowkey = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "rowkey", true) == 0).Value;
                if (rowkey == null)
                {
                    dynamic data = await req.Content.ReadAsAsync<object>();
                    rowkey = data?.rowkey;
                }
                if (rowkey != null)
                {
                    BlobClient blobClient = new BlobClient(CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
                    file = blobClient.getFile(rowkey);
                    file.fileStr_VC = blobClient.getFiletoStr(file.filename_VC);
                }
                return rowkey == null
                    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a rowkey on the query string or in the request body")
                    : req.CreateResponse(HttpStatusCode.OK, file);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, $"Guru Meditation #{ex.Message}");
            }
        }
    }
}
