using ConsumeFtpFiles.Enums;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFtpFiles.Models
{
    class LogEntryEntity : TableEntity
    {
        public LogEntryEntity(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public LogEntryEntity() { }

        public DateTime dateTime_VC { get; set; }
        public string fileName_VC { get; set; }
        public int status_VC { get; set; }
    }
}
