using System.IO;
using System.Net;

namespace LogScreen.Managers
{
    public class FTPUploader
    {
        public void UploadFile(string filePath, string ftpUrl, string username, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create(ftpUrl + "/" + Path.GetFileName(filePath));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(username, password);

            using (var fs = File.OpenRead(filePath))
            using (var stream = request.GetRequestStream())
            {
                fs.CopyTo(stream);
            }
        }
    }
}






