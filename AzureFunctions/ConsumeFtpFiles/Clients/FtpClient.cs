using ConsumeFtpFiles.Models;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeFtpFiles.Clients
{
    class FtpClient
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;

        public FtpClient(string hostIP, string userName, string password) { host = hostIP; user = userName; pass = password; }

        public FileEntry putToBlob(string remoteFile, BlobClient blobClient)
        {
            try
            {
                var fileEntry = new FileEntry();
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                try
                {
                    using (var reader = ReaderFactory.Open(ftpStream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    reader.WriteEntryTo(ms);
                                    fileEntry = blobClient.UploadBlob(reader.Entry.Key, ms);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                return fileEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] directoryListSimple(string directory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string directoryRaw = null;
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { throw ex; }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                try
                {
                    string[] directoryList = directoryRaw.Split("|".ToCharArray());
                    return directoryList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

