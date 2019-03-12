using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

// ReSharper disable once IdentifierTypo
namespace GirlsFrontline_Downloader
{
    internal static class Program
    {
        private static async Task Main()
        {
            var dirPath = Environment.CurrentDirectory;
            dirPath = Path.Combine(Directory.GetParent(Directory.GetParent(dirPath).ToString()) + @"\data\");
            Console.WriteLine("Please choose which server to download (en/cn):");
            string serverInput;
            do
            {
                serverInput = Console.ReadLine();
            } while (serverInput != "en" && serverInput != "cn");
            var filePath = Path.Combine(dirPath + @"\wiki-" + serverInput + ".html");
            /*try
            {*/
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream, Encoding.Default);
            fileStream.Seek(0, SeekOrigin.Begin);

            string serverName = null;
            if (serverInput == "cn") serverName = "http://www.gfwiki.org";
            if (serverInput == "en") serverName = "https://www.gfwiki.com";

            var content = streamReader.ReadLine();
            var count = 0;
            //var myWebClient = new WebClient();
            /*
            var handler = new WebRequestHandler();
            X509Certificate2 certificate = GetMyX509Certificate();
            handler.ClientCertificates.Add(certificate);
            */
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            while (content != null)
            {
                var uriSplit = content.Split(Convert.ToChar("\""));
                var url = serverName + uriSplit[1].Trim();
                var localPath = url.Split('/')[6].Trim();
                var downloadPath = Path.Combine(Path.Combine(dirPath + @"download\" + serverInput + @"\") +
                                                HttpUtility.UrlDecode(localPath, Encoding.UTF8));
                var picBool = true;
                if (serverInput == "cn") {
                    picBool = (localPath.Substring(0, 3) == "Pic");
                }
                if (picBool && File.Exists(downloadPath) == false)
                {
                    //myWebClient.DownloadFile(url, downloadPath);
                    var picContent = await client.GetByteArrayAsync(url);
                    var strPicContent = Encoding.Default.GetString(picContent);
                    if (strPicContent.Substring(0, 15) == "<!DOCTYPE html>") {
                        Console.WriteLine("[ERROR] " + url);
                    } else {
                        count++;
                        var fs = new FileStream(downloadPath, FileMode.Create);
                        fs.Write(picContent, 0, picContent.Length);
                        Console.WriteLine("[NO." + count + " Download] " + url);
                    }
                }
                content = streamReader.ReadLine();
            }
            fileStream.Close();
            streamReader.Close();
            /*}
            catch
            {
                // ignored
            }
            finally
            {
                fileStream?.Close();
                streamReader?.Close();
            }*/
        }

        /*
        private static void Downloader(string url, string filepath)
        {

        }
        */
    }
}
