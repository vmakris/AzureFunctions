using System;
using System.Linq;
using ConsumeFtpFiles.Clients;
using ConsumeFtpFiles.Enums;
using ConsumeFtpFiles.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace ConsumeFtpFiles
{
    public static class GetFtpFilesFunction
    {
        [FunctionName("GetFtpFilesFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            try
            {
                if (Convert.ToBoolean(Environment.GetEnvironmentVariable("isRunning")))
                {
                    log.Info($"Function is already running: {DateTime.Now}");
                }
                else
                {
                    Environment.SetEnvironmentVariable("isRunning", "true");
                    FtpClient ftpClient = new FtpClient(Environment.GetEnvironmentVariable("ftpURL"), Environment.GetEnvironmentVariable("ftpUserName"), Environment.GetEnvironmentVariable("ftpPassWord"));
                    BlobClient blobClient = new BlobClient(CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

                    var completedFiles = blobClient.getAllLogs().Select(x => x.fileName_VC).ToList();
                    var fileList = ftpClient.directoryListSimple("");
                    foreach (var file in fileList)
                    {
                        if (file.Length > 0 && !completedFiles.Contains(file))
                        {
                            ftpClient.putToBlob(file, blobClient);
                            blobClient.writeLog(new LogEntryEntity(String.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), Guid.NewGuid().ToString())
                            {
                                dateTime_VC = DateTime.Now,
                                fileName_VC = file,
                                status_VC = (int)LogStatus.Completed
                            });
                            log.Info("filename:" + file);
                        }
                    }
                    ftpClient = null;
                    Environment.SetEnvironmentVariable("isRunning", "false");
                    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                log.Info($"Exception: {ex.Message}");
                Environment.SetEnvironmentVariable("isRunning", "false");
                throw ex;
            }
        }
    }
}
