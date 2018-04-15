using ConsumeFtpFiles.Enums;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFtpFiles.Models
{
    class FileEntryEntity : TableEntity
    {
        public FileEntryEntity(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public FileEntryEntity() { }

        public DateTime dateTime_VC { get; set; }
        public string fileurl_VC { get; set; }
        public string filename_VC { get; set; }
        public string filetype_VC { get; set; }
        public string fileStr_VC { get; set; }
    }
}

