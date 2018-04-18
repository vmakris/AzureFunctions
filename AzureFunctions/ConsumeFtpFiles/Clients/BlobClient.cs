using ConsumeFtpFiles.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFtpFiles.Clients
{
    class BlobClient
    {
        private CloudStorageAccount _account = null;
        private string _folder = "ftpdata";
        private string _queue = "filelogs";

        public BlobClient(CloudStorageAccount account) { _account = account; }

        public FileEntry UploadBlob(string file, MemoryStream stream)
        {
            try
            {
                var client = _account.CreateCloudBlobClient();
                var cloudBlobContainer = client.GetContainerReference(_folder);
                cloudBlobContainer.CreateIfNotExists();
                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(file);
                stream.Position = 0;
                blob.UploadFromStream(stream);
                var log = new FileEntry(String.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), Guid.NewGuid().ToString())
                {
                    dateTime_VC = DateTime.Now,
                    fileurl_VC = blob.StorageUri.PrimaryUri.OriginalString,
                    filename_VC = file,
                    filetype_VC = System.IO.Path.GetExtension(file)
                };
                writeQueueMessage(log, _queue);
                return log;
            }
            catch (Exception ex) { throw ex; }
        }

        public IEnumerable<FileEntryEntity> getFiles()
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference("FunctionFiles");
                table.CreateIfNotExists();
                var fileList = new List<FileEntryEntity>();
                var query = new TableQuery<FileEntryEntity>().Take(50);
                return table.ExecuteQuery(query);
            }
            catch (Exception ex) { throw ex; }
        }

        public FileEntryEntity getFile(string rowkey)
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference("FunctionFiles");
                table.CreateIfNotExists();
                var file = new FileEntryEntity();
                var query = new TableQuery<FileEntryEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowkey));
                return table.ExecuteQuery(query).FirstOrDefault();
            }
            catch (Exception ex) { throw ex; }
        }

        public string getFiletoStr(string name)
        {
            try
            {
                var client = _account.CreateCloudBlobClient();
                var cloudBlobContainer = client.GetContainerReference(_folder);
                var blob = cloudBlobContainer.GetBlobReference(name);
                if (blob.Exists())
                {
                    var xmlStr = "";
                    using (StreamReader reader = new StreamReader(blob.OpenRead()))
                    {
                        xmlStr = reader.ReadToEnd();
                    }
                    return xmlStr;
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
        }

        public bool writeLog(LogEntryEntity log)
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference("FunctionLogs");
                table.CreateIfNotExists();
                TableOperation insertOperation = TableOperation.Insert(log);
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex) { throw ex; }
        }

        public bool writeQueueMessage(FileEntry log, string queueName)
        {
            try
            {
                var queueClient = _account.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(queueName);
                queue.CreateIfNotExists();
                queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(log)));
                return true;
            }
            catch (Exception ex) { throw ex; }
        }

        public bool writeLogFile(FileEntryEntity file)
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference("FunctionFiles");
                table.CreateIfNotExists();
                TableOperation insertOperation = TableOperation.Insert(file);
                table.Execute(insertOperation);
                return true;
            }
            catch (Exception ex) { throw ex; }
        }

        public IEnumerable<LogEntryEntity> getAllLogs()
        {
            try
            {
                var tableClient = _account.CreateCloudTableClient();
                var table = tableClient.GetTableReference("FunctionLogs");
                table.CreateIfNotExists();
                var logList = new List<LogEntryEntity>();
                var query = new TableQuery<LogEntryEntity>();
                return table.ExecuteQuery(query);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
