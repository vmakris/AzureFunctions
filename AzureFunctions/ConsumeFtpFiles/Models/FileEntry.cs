using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFtpFiles.Models
{
    [Serializable]
    class FileEntry
    {
        public FileEntry(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public FileEntry() { }

        public DateTime dateTime_VC { get; set; }
        public string fileurl_VC { get; set; }
        public string filename_VC { get; set; }
        public string filetype_VC { get; set; }
        public string fileStr_VC { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}
